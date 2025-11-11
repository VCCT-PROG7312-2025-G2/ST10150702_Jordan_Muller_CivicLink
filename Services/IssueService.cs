using CivicLink.Models;
using CivicLink.DataStructures;


// Business logic service for issue management using custom data structures (no primitive arrays/lists)
namespace CivicLink.Services
{
    public interface IIssueService
    {
        Task<int> CreateIssueAsync(Issue issue);
        Task<Issue> GetIssueByIdAsync(int id);
        Task<IEnumerable<Issue>> GetAllIssuesAsync();
        Task<IEnumerable<Issue>> GetIssuesByPriorityAsync();
        Task<bool> UpdateIssueAsync(Issue issue);
        Task<bool> DeleteIssueAsync(int id);
        Task<IEnumerable<string>> GetRecentActivitiesAsync(int count = 10);
        void LogActivity(string activity);
    }

    public class IssueService : IIssueService
    {
        private readonly IssueLinkedList issueStorage;
        private readonly IssuePriorityQueue priorityQueue;
        private readonly ActivityStack activityLog;
        private static int nextId = 1;

        public IssueService()
        {
            issueStorage = new IssueLinkedList();
            priorityQueue = new IssuePriorityQueue();
            activityLog = new ActivityStack();

            // Initialize with sample data for demonstration
            InitializeSampleData();
        }

        public async Task<int> CreateIssueAsync(Issue issue)
        {
            return await Task.Run(() =>
            {
                issue.Id = nextId++;
                issue.CreatedAt = DateTime.Now;
                issue.Status = IssueStatus.Submitted;

                issueStorage.Add(issue);
                priorityQueue.Enqueue(issue);

                LogActivity($"New issue created: {issue.Title} (ID: {issue.Id})");

                return issue.Id;
            });
        }

        public async Task<Issue> GetIssueByIdAsync(int id)
        {
            return await Task.Run(() => issueStorage.GetById(id));
        }

        public async Task<IEnumerable<Issue>> GetAllIssuesAsync()
        {
            return await Task.Run(() => issueStorage.GetAll().OrderByDescending(i => i.CreatedAt));
        }

        public async Task<IEnumerable<Issue>> GetIssuesByPriorityAsync()
        {
            return await Task.Run(() => priorityQueue.GetAll());
        }

        public async Task<bool> UpdateIssueAsync(Issue issue)
        {
            return await Task.Run(() =>
            {
                var existingIssue = issueStorage.GetById(issue.Id);
                if (existingIssue != null)
                {
                    existingIssue.Title = issue.Title;
                    existingIssue.Description = issue.Description;
                    existingIssue.Category = issue.Category;
                    existingIssue.Location = issue.Location;
                    existingIssue.Priority = issue.Priority;
                    existingIssue.Status = issue.Status;
                    existingIssue.UpdatedAt = DateTime.Now;

                    LogActivity($"Issue updated: {issue.Title} (ID: {issue.Id})");
                    return true;
                }
                return false;
            });
        }

        public async Task<bool> DeleteIssueAsync(int id)
        {
            return await Task.Run(() =>
            {
                var issue = issueStorage.GetById(id);
                if (issue != null)
                {
                    var removed = issueStorage.Remove(id);
                    if (removed)
                    {
                        LogActivity($"Issue deleted: {issue.Title} (ID: {id})");
                    }
                    return removed;
                }
                return false;
            });
        }

        public async Task<IEnumerable<string>> GetRecentActivitiesAsync(int count = 10)
        {
            return await Task.Run(() => activityLog.GetRecent(count));
        }

        public void LogActivity(string activity)
        {
            activityLog.Push(activity);
        }

        // Felt that having more sample data would help with UX/UI as well as Testing
        // Gave Claude my original 2 samples and asked for 8 more similar ones as creating sample data is slow and tedious
        private void InitializeSampleData()
        {
            // Add sample issues for demonstration - expanded to 10 for better testing
            var sampleIssues = new[]
            {
        new Issue
        {
            Id = nextId++,
            Title = "Broken streetlight on Main Road",
            Description = "The streetlight near the intersection is not working, creating safety concerns.",
            Category = IssueCategory.PublicSafety,
            Location = "Main Road & Oak Street Intersection, Cape Town",
            Priority = IssuePriority.High,
            Status = IssueStatus.InProgress,
            CreatedAt = DateTime.Now.AddDays(-2),
            ContactName = "John Smith",
            ContactEmail = "john@example.com",
            ContactPhone = "021-555-0123"
        },
        new Issue
        {
            Id = nextId++,
            Title = "Water leak in residential area",
            Description = "Continuous water leak causing flooding in the street.",
            Category = IssueCategory.WaterAndSanitation,
            Location = "Sunset Avenue, Camps Bay",
            Priority = IssuePriority.Critical,
            Status = IssueStatus.InReview,
            CreatedAt = DateTime.Now.AddDays(-1),
            ContactName = "Sarah Johnson",
            ContactEmail = "sarah@example.com",
            ContactPhone = "021-555-0456"
        },
        new Issue
        {
            Id = nextId++,
            Title = "Pothole on Highway M3",
            Description = "Large pothole causing damage to vehicles and creating hazardous driving conditions.",
            Category = IssueCategory.RoadsAndTransport,
            Location = "M3 Highway near Kirstenbosch",
            Priority = IssuePriority.High,
            Status = IssueStatus.Submitted,
            CreatedAt = DateTime.Now.AddDays(-5),
            ContactName = "Michael Chen",
            ContactEmail = "mchen@example.com",
            ContactPhone = "021-555-0789"
        },
        new Issue
        {
            Id = nextId++,
            Title = "Illegal dumping in park",
            Description = "Large amount of waste dumped illegally in the public park area.",
            Category = IssueCategory.WasteManagement,
            Location = "Green Point Urban Park",
            Priority = IssuePriority.Medium,
            Status = IssueStatus.InProgress,
            CreatedAt = DateTime.Now.AddDays(-3),
            ContactName = "Ayanda Mthembu",
            ContactEmail = "ayanda@example.com",
            ContactPhone = "021-555-0234"
        },
        new Issue
        {
            Id = nextId++,
            Title = "Power outage in neighborhood",
            Description = "Entire neighborhood experiencing power outage for over 6 hours.",
            Category = IssueCategory.ElectricityAndPower,
            Location = "Sea Point, Beach Road area",
            Priority = IssuePriority.Critical,
            Status = IssueStatus.InProgress,
            CreatedAt = DateTime.Now.AddHours(-8),
            ContactName = "David Botha",
            ContactEmail = "dbotha@example.com",
            ContactPhone = "021-555-0567"
        },
        new Issue
        {
            Id = nextId++,
            Title = "Overgrown vegetation blocking road sign",
            Description = "Trees and bushes have grown and are completely blocking the stop sign.",
            Category = IssueCategory.ParksAndRecreation,
            Location = "Constantia Main Road",
            Priority = IssuePriority.Medium,
            Status = IssueStatus.Submitted,
            CreatedAt = DateTime.Now.AddDays(-7),
            ContactName = "Lisa van der Merwe",
            ContactEmail = "lisa@example.com",
            ContactPhone = "021-555-0890"
        },
        new Issue
        {
            Id = nextId++,
            Title = "Broken traffic light at busy intersection",
            Description = "Traffic light has been malfunctioning causing traffic congestion and near-accidents.",
            Category = IssueCategory.RoadsAndTransport,
            Location = "Main Road & Kloof Street, Gardens",
            Priority = IssuePriority.Critical,
            Status = IssueStatus.InReview,
            CreatedAt = DateTime.Now.AddDays(-1),
            ContactName = "Thabo Ndlovu",
            ContactEmail = "thabo@example.com",
            ContactPhone = "021-555-0345"
        },
        new Issue
        {
            Id = nextId++,
            Title = "Blocked storm drain causing flooding",
            Description = "Storm drain is blocked with debris, causing water to pool during rain.",
            Category = IssueCategory.WaterAndSanitation,
            Location = "Rondebosch Main Road",
            Priority = IssuePriority.High,
            Status = IssueStatus.InProgress,
            CreatedAt = DateTime.Now.AddDays(-4),
            ContactName = "Emma Williams",
            ContactEmail = "emma@example.com",
            ContactPhone = "021-555-0678"
        },
        new Issue
        {
            Id = nextId++,
            Title = "Graffiti on public building",
            Description = "Extensive graffiti vandalism on the side of the community center.",
            Category = IssueCategory.PublicSafety,
            Location = "Woodstock Community Center",
            Priority = IssuePriority.Low,
            Status = IssueStatus.Submitted,
            CreatedAt = DateTime.Now.AddDays(-10),
            ContactName = "Peter Stewart",
            ContactEmail = "pstewart@example.com",
            ContactPhone = "021-555-0901"
        },
        new Issue
        {
            Id = nextId++,
            Title = "Damaged playground equipment",
            Description = "Swing set has broken chains and poses safety risk to children.",
            Category = IssueCategory.ParksAndRecreation,
            Location = "Claremont Park Playground",
            Priority = IssuePriority.Medium,
            Status = IssueStatus.Resolved,
            CreatedAt = DateTime.Now.AddDays(-15),
            UpdatedAt = DateTime.Now.AddDays(-2),
            ContactName = "Nomsa Dlamini",
            ContactEmail = "nomsa@example.com",
            ContactPhone = "021-555-0123"
        }
    };

            foreach (var issue in sampleIssues)
            {
                issueStorage.Add(issue);
                priorityQueue.Enqueue(issue);
                LogActivity($"Sample issue added: {issue.Title} (ID: {issue.Id})");
            }
        }
    }
}