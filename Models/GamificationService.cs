using CivicLink.Models;
using CivicLink.DataStructures;

namespace CivicLink.Services
{
    public interface IGamificationService
    {
        Task<UserEngagement> GetUserEngagementAsync(string userId);
        Task<UserEngagement> UpdateUserEngagementAsync(string userId, int pointsEarned, string activity);
        Task<List<Badge>> GetAvailableBadgesAsync();
        Task<List<Badge>> CheckForNewBadgesAsync(UserEngagement userEngagement);
    }

    public class GamificationService : IGamificationService
    {
        private readonly Dictionary<string, UserEngagement> userEngagements;
        private readonly List<Badge> availableBadges;
        private readonly ActivityStack userActivityLog;

        public GamificationService()
        {
            userEngagements = new Dictionary<string, UserEngagement>();
            userActivityLog = new ActivityStack(100);
            availableBadges = InitializeBadges();
        }

        public async Task<UserEngagement> GetUserEngagementAsync(string userId)
        {
            return await Task.Run(() =>
            {
                if (!userEngagements.ContainsKey(userId))
                {
                    userEngagements[userId] = new UserEngagement
                    {
                        Id = userEngagements.Count + 1,
                        UserId = userId,
                        Points = 0,
                        Level = 1,
                        LastActivity = DateTime.Now,
                        IssuesReported = 0,
                        IssuesResolved = 0,
                        IsActive = true
                    };
                }
                return userEngagements[userId];
            });
        }

        public async Task<UserEngagement> UpdateUserEngagementAsync(string userId, int pointsEarned, string activity)
        {
            return await Task.Run(async () =>
            {
                var engagement = await GetUserEngagementAsync(userId);
                engagement.Points += pointsEarned;
                engagement.LastActivity = DateTime.Now;

                // Update level based on points
                engagement.Level = CalculateLevel(engagement.Points);

                // Update activity counters
                if (activity.Contains("issue reported"))
                    engagement.IssuesReported++;
                else if (activity.Contains("issue resolved"))
                    engagement.IssuesResolved++;

                // Check for new badges
                var newBadges = await CheckForNewBadgesAsync(engagement);
                foreach (var badge in newBadges)
                {
                    if (!engagement.Badges.Any(b => b.Type == badge.Type))
                    {
                        badge.EarnedAt = DateTime.Now;
                        badge.IsUnlocked = true;
                        engagement.Badges.Add(badge);
                        userActivityLog.Push($"Badge earned: {badge.Name}");
                    }
                }

                userActivityLog.Push($"User {userId}: {activity} (+{pointsEarned} points)");
                return engagement;
            });
        }

        public async Task<List<Badge>> GetAvailableBadgesAsync()
        {
            return await Task.Run(() => availableBadges.ToList());
        }

        public async Task<List<Badge>> CheckForNewBadgesAsync(UserEngagement userEngagement)
        {
            return await Task.Run(() =>
            {
                var newBadges = new List<Badge>();

                foreach (var badge in availableBadges)
                {
                    if (!userEngagement.Badges.Any(b => b.Type == badge.Type))
                    {
                        bool shouldEarn = badge.Type switch
                        {
                            BadgeType.FirstReport => userEngagement.IssuesReported >= 1,
                            BadgeType.CommunityHelper => userEngagement.IssuesReported >= 5,
                            BadgeType.ProblemSolver => userEngagement.IssuesReported >= 10,
                            BadgeType.CivicChampion => userEngagement.IssuesReported >= 25,
                            BadgeType.PowerUser => userEngagement.Points >= 500,
                            BadgeType.SafetyAdvocate => userEngagement.IssuesReported >= 15 && userEngagement.Points >= 300,
                            BadgeType.EnvironmentalGuardian => userEngagement.IssuesReported >= 20 && userEngagement.Points >= 400,
                            _ => false
                        };

                        if (shouldEarn)
                        {
                            newBadges.Add(new Badge
                            {
                                Id = badge.Id,
                                Name = badge.Name,
                                Description = badge.Description,
                                IconClass = badge.IconClass,
                                Type = badge.Type,
                                PointsRequired = badge.PointsRequired,
                                EarnedAt = DateTime.Now,
                                IsUnlocked = true
                            });
                        }
                    }
                }

                return newBadges;
            });
        }

        private int CalculateLevel(int points)
        {
            if (points < 50) return 1;
            if (points < 150) return 2;
            if (points < 300) return 3;
            if (points < 500) return 4;
            if (points < 750) return 5;
            if (points < 1000) return 6;
            return 7;
        }

        private List<Badge> InitializeBadges()
        {
            return new List<Badge>
            {
                new Badge
                {
                    Id = 1,
                    Name = "First Reporter",
                    Description = "Submitted your first issue report",
                    IconClass = "fas fa-star",
                    Type = BadgeType.FirstReport,
                    PointsRequired = 0
                },
                new Badge
                {
                    Id = 2,
                    Name = "Community Helper",
                    Description = "Reported 5 community issues",
                    IconClass = "fas fa-hands-helping",
                    Type = BadgeType.CommunityHelper,
                    PointsRequired = 100
                },
                new Badge
                {
                    Id = 3,
                    Name = "Problem Solver",
                    Description = "Reported 10 issues to help improve the community",
                    IconClass = "fas fa-lightbulb",
                    Type = BadgeType.ProblemSolver,
                    PointsRequired = 200
                },
                new Badge
                {
                    Id = 4,
                    Name = "Civic Champion",
                    Description = "Reported 25 issues - you're a true civic champion!",
                    IconClass = "fas fa-trophy",
                    Type = BadgeType.CivicChampion,
                    PointsRequired = 500
                },
                new Badge
                {
                    Id = 5,
                    Name = "Power User",
                    Description = "Earned 500+ engagement points",
                    IconClass = "fas fa-bolt",
                    Type = BadgeType.PowerUser,
                    PointsRequired = 500
                },
                new Badge
                {
                    Id = 6,
                    Name = "Safety Advocate",
                    Description = "Dedicated to improving community safety",
                    IconClass = "fas fa-shield-alt",
                    Type = BadgeType.SafetyAdvocate,
                    PointsRequired = 300
                },
                new Badge
                {
                    Id = 7,
                    Name = "Environmental Guardian",
                    Description = "Protecting our environment through active reporting",
                    IconClass = "fas fa-leaf",
                    Type = BadgeType.EnvironmentalGuardian,
                    PointsRequired = 400
                }
            };
        }
    }
}