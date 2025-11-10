// Business logic for Part 3: Service Request Status tracking and management
// Integrates BST, AVL Tree, Heap, and Graph structures for efficient request handling
using CivicLink.Models;
using CivicLink.DataStructures;

namespace CivicLink.Services
{
    /* 
     * Summary: Service layer for Part 3 Service Request Status feature.
     * Manages service requests using advanced data structures including
     * BST/AVL for searching, Heap for priorities, and Graph for relationships.
     * Provides efficient tracking, searching, and relationship analysis.
     */

    public interface IServiceRequestService
    {
        Task<Issue> GetRequestByIdAsync(int id);
        Task<List<Issue>> GetAllRequestsAsync();
        Task<List<Issue>> GetRequestsByPriorityAsync();
        Task<List<Issue>> GetRelatedRequestsAsync(int id);
        Task<List<ServiceRequestEdge>> GetRequestRelationshipsAsync(int id);
        Task<List<ServiceRequestEdge>> GetMinimumSpanningTreeAsync();
        Task<Dictionary<string, int>> GetRequestStatisticsAsync();
        Task RebuildDataStructuresAsync();
    }

    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IIssueService _issueService;

        // Core data structures for Part 3
        private IssueSearchTree bst;
        private AVLSearchTree avlTree;
        private ServiceRequestMinHeap priorityHeap;
        private ServiceRequestGraph relationshipGraph;

        public ServiceRequestService(IIssueService issueService)
        {
            _issueService = issueService;

            // Initialize data structures
            bst = new IssueSearchTree();
            avlTree = new AVLSearchTree();
            priorityHeap = new ServiceRequestMinHeap();
            relationshipGraph = new ServiceRequestGraph();

            // Build initial structures asynchronously
            Task.Run(() => RebuildDataStructuresAsync());
        }

        // Rebuild all data structures from current issues
        // Called when data changes or on initialization
        public async Task RebuildDataStructuresAsync()
        {
            await Task.Run(async () =>
            {
                // Get all issues from the issue service
                var allIssues = await _issueService.GetAllIssuesAsync();
                var issueList = allIssues.ToList();

                // Clear existing structures
                bst = new IssueSearchTree();
                avlTree = new AVLSearchTree();
                priorityHeap = new ServiceRequestMinHeap();
                relationshipGraph = new ServiceRequestGraph();

                // Populate BST and AVL Tree for efficient searching
                foreach (var issue in issueList)
                {
                    bst.Insert(issue);
                    avlTree.Insert(issue);
                    priorityHeap.Insert(issue);
                    relationshipGraph.AddVertex(issue);
                }

                // Build graph edges based on relationships
                BuildGraphRelationships(issueList);
            });
        }

        /* 
         * ============================================
         * Graph relationship logic assisted using research from: https://www.youtube.com/watch?v=LRKPZi6oMhc&t=2s
         * Calculates similarity weights based on multiple factors
         * ============================================
         */
        // Build edges in the graph based on issue similarities
        // Issues are related if they share location, category, or timing
        private void BuildGraphRelationships(List<Issue> issues)
        {
            for (int i = 0; i < issues.Count; i++)
            {
                for (int j = i + 1; j < issues.Count; j++)
                {
                    var issue1 = issues[i];
                    var issue2 = issues[j];

                    // Calculate relationship weight based on similarity
                    double weight = CalculateRelationshipWeight(issue1, issue2);

                    // Only create edge if issues are related (weight below threshold)
                    if (weight < 10.0)
                    {
                        string relationshipType = DetermineRelationshipType(issue1, issue2);

                        relationshipGraph.AddEdge(issue1.Id, issue2.Id, weight, relationshipType);
                        relationshipGraph.AddEdge(issue2.Id, issue1.Id, weight, relationshipType);
                    }
                }
            }
        }

        // Calculate how related two issues are (lower weight = more related)
        private double CalculateRelationshipWeight(Issue issue1, Issue issue2)
        {
            double weight = 10.0;

            // Same category reduces weight significantly
            if (issue1.Category == issue2.Category)
            {
                weight -= 3.0;
            }

            // Similar location reduces weight
            if (AreSimilarLocations(issue1.Location, issue2.Location))
            {
                weight -= 3.0;
            }

            // Similar timing reduces weight
            var timeDiff = Math.Abs((issue1.CreatedAt - issue2.CreatedAt).TotalDays);
            if (timeDiff < 7)
            {
                weight -= 2.0;
            }
            else if (timeDiff < 30)
            {
                weight -= 1.0;
            }

            // Same priority reduces weight slightly
            if (issue1.Priority == issue2.Priority)
            {
                weight -= 1.0;
            }

            return Math.Max(1.0, weight);
        }

        // Determine the type of relationship between two issues
        private string DetermineRelationshipType(Issue issue1, Issue issue2)
        {
            if (issue1.Category == issue2.Category && AreSimilarLocations(issue1.Location, issue2.Location))
            {
                return "Same Area & Category";
            }
            else if (issue1.Category == issue2.Category)
            {
                return "Same Category";
            }
            else if (AreSimilarLocations(issue1.Location, issue2.Location))
            {
                return "Same Area";
            }
            else if (Math.Abs((issue1.CreatedAt - issue2.CreatedAt).TotalDays) < 7)
            {
                return "Similar Time";
            }
            return "Related";
        }

        // Check if two locations are similar
        // Simple string matching - could be enhanced with actual geolocation
        private bool AreSimilarLocations(string location1, string location2)
        {
            var loc1Words = location1.ToLower().Split(new[] { ' ', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var loc2Words = location2.ToLower().Split(new[] { ' ', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);

            return loc1Words.Intersect(loc2Words).Any();
        }

        // Get a single request by ID using BST for O(log n) search
        public async Task<Issue> GetRequestByIdAsync(int id)
        {
            return await Task.Run(() =>
            {
                // Try BST first
                var result = bst.Search(id);

                // Fallback to AVL if BST fails
                if (result == null)
                {
                    result = avlTree.Search(id);
                }

                return result;
            });
        }

        // Get all requests sorted by ID using BST in-order traversal
        public async Task<List<Issue>> GetAllRequestsAsync()
        {
            return await Task.Run(() => bst.GetAllSorted());
        }

        // Get requests ordered by priority using Min-Heap
        public async Task<List<Issue>> GetRequestsByPriorityAsync()
        {
            return await Task.Run(() => priorityHeap.GetAllByPriority());
        }

        // Get requests related to a specific request using Graph
        public async Task<List<Issue>> GetRelatedRequestsAsync(int id)
        {
            return await Task.Run(() => relationshipGraph.GetRelatedRequests(id, 5));
        }

        // Get the relationship edges for a specific request
        public async Task<List<ServiceRequestEdge>> GetRequestRelationshipsAsync(int id)
        {
            return await Task.Run(() => relationshipGraph.GetEdges(id));
        }

        // Get the Minimum Spanning Tree of all request relationships
        // This shows the most efficient connections between related requests
        public async Task<List<ServiceRequestEdge>> GetMinimumSpanningTreeAsync()
        {
            return await Task.Run(() => relationshipGraph.GetMinimumSpanningTree());
        }

        // Get statistics about service requests
        public async Task<Dictionary<string, int>> GetRequestStatisticsAsync()
        {
            return await Task.Run(async () =>
            {
                var allIssues = await GetAllRequestsAsync();

                var stats = new Dictionary<string, int>
                {
                    ["Total"] = allIssues.Count,
                    ["Submitted"] = allIssues.Count(i => i.Status == IssueStatus.Submitted),
                    ["InReview"] = allIssues.Count(i => i.Status == IssueStatus.InReview),
                    ["InProgress"] = allIssues.Count(i => i.Status == IssueStatus.InProgress),
                    ["Resolved"] = allIssues.Count(i => i.Status == IssueStatus.Resolved),
                    ["Closed"] = allIssues.Count(i => i.Status == IssueStatus.Closed),
                    ["Critical"] = allIssues.Count(i => i.Priority == IssuePriority.Critical),
                    ["High"] = allIssues.Count(i => i.Priority == IssuePriority.High),
                    ["BSTNodes"] = bst.Count,
                    ["AVLNodes"] = avlTree.Count,
                    ["HeapSize"] = priorityHeap.Count
                };

                return stats;
            });
        }
    }
}