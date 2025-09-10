using System.ComponentModel.DataAnnotations;

namespace CivicLink.Models
{
    public class Issue
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Issue title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public IssueCategory Category { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        public string Location { get; set; }

        public IssuePriority Priority { get; set; }

        public IssueStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<string> AttachmentPaths { get; set; } = new List<string>();

        public string ContactName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string ContactEmail { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string ContactPhone { get; set; }
    }

    public enum IssueCategory
    {
        WaterAndSanitation,
        RoadsAndTransport,
        ElectricityAndPower,
        WasteManagement,
        PublicSafety,
        ParksAndRecreation,
        Housing,
        BusinessLicensing,
        Other
    }

    public enum IssuePriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum IssueStatus
    {
        Submitted,
        InReview,
        InProgress,
        Resolved,
        Closed
    }
}