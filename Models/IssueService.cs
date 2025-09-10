using CivicLink.Models;
using CivicLink.DataStructures;

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

        private void InitializeSampleData()
        {
            // Add some sample issues for demonstration
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