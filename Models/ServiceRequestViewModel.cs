// View model for Part 3: Service Request Status page display
using CivicLink.DataStructures;

namespace CivicLink.Models
{
    /* 
     * Summary: View model for the Service Request Status page.
     * Contains all data needed to display service requests, their relationships,
     * statistics, and search results for Part 3.
     * Nothing here was too difficult, I did get tripped up with the Dictionary as well as a solution for flagging what we're showing
     */

    public class ServiceRequestViewModel
    {
        // Main list of service requests to display
        public List<Issue> ServiceRequests { get; set; } = new List<Issue>();

        // Related requests for selected issue
        public List<Issue> RelatedRequests { get; set; } = new List<Issue>();

        // Graph edges showing relationships
        public List<ServiceRequestEdge> Relationships { get; set; } = new List<ServiceRequestEdge>();

        // Minimum Spanning Tree edges for optimal connections
        public List<ServiceRequestEdge> MinimumSpanningTree { get; set; } = new List<ServiceRequestEdge>();

        // Statistics for dashboard display
        public Dictionary<string, int> Statistics { get; set; } = new Dictionary<string, int>();

        // Currently selected/viewed request
        public Issue SelectedRequest { get; set; }

        // Search query if user searched
        public string SearchQuery { get; set; }

        // Filter options
        public IssueStatus? FilterStatus { get; set; }
        public IssueCategory? FilterCategory { get; set; }
        public IssuePriority? FilterPriority { get; set; }

        // Sort option
        public string SortBy { get; set; }

        // Flag indicating if we're showing priority-sorted view
        public bool ShowPriorityView { get; set; }

        // Flag indicating if we're showing graph relationships
        public bool ShowGraphView { get; set; }
    }
}