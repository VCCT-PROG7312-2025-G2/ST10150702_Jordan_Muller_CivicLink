namespace CivicLink.Models
{
    public class UserEngagement
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }
        public List<Badge> Badges { get; set; } = new List<Badge>();
        public DateTime LastActivity { get; set; }
        public int IssuesReported { get; set; }
        public int IssuesResolved { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Badge
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }
        public BadgeType Type { get; set; }
        public int PointsRequired { get; set; }
        public DateTime EarnedAt { get; set; }
        public bool IsUnlocked { get; set; }
    }

    public enum BadgeType
    {
        FirstReport,
        CommunityHelper,
        ProblemSolver,
        CivicChampion,
        PowerUser,
        SafetyAdvocate,
        EnvironmentalGuardian
    }

    public class ReportIssueViewModel
    {
        public Issue Issue { get; set; } = new Issue();
        public UserEngagement UserEngagement { get; set; }
        public List<Badge> AvailableBadges { get; set; } = new List<Badge>();
        public bool ShowSuccessMessage { get; set; }
        public string SuccessMessage { get; set; }
    }
}