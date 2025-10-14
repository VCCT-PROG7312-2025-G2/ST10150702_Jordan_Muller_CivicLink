// Core models for local events and announcements with categories and date management
using System.ComponentModel.DataAnnotations;

namespace CivicLink.Models
{
    /* 
     * Summary: This file contains the Event model used to store information about
     * local community events and municipal announcements. It includes properties
     * for event details, categorization, and date management.
     */

    public class Event
    {
        // Unique ID
        public int Id { get; set; }

        // Event name
        [Required(ErrorMessage = "Event name is required")]
        [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
        public string Name { get; set; }

        // Description of the event
        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        // Type of Event
        [Required(ErrorMessage = "Category is required")]
        public EventCategory Category { get; set; }

        // Event Location
        [Required(ErrorMessage = "Location is required")]
        [StringLength(200)]
        public string Location { get; set; }

        // Start time
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        // End time, which I made not compulsory as events might not have an end date.
        public DateTime? EndDate { get; set; }

        // Contact person 
        public string ContactPerson { get; set; }

        // Phone number for any questions
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string ContactPhone { get; set; }

        // Is or isnt free?
        public bool IsFree { get; set; }

        // If not free, the cost
        public decimal? Cost { get; set; }

        // Date and time of event creation in the system
        public DateTime CreatedAt { get; set; }

        // is this event still active?
        public bool IsActive { get; set; } = true;

    }

    // Types of community events
    public enum EventCategory
    {
        CommunityMeeting,
        TownHall,
        PublicHearing,
        Recreation,
        Education,
        Health,
        Safety,
        Environment,
        Arts,
        Sports,
        Festival,
        Workshop,
        Other
    }

    // Announcements which are different from events
    public class Announcement
    {
        // Classic unique ID
        public int Id { get; set; }

        // Announcement title
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        // Content for the announcement
        [Required]
        [StringLength(2000)]
        public string Content { get; set; }

        // Publish date of announcment
        public DateTime PublishedDate { get; set; }

        // Announcment priority
        public AnnouncementPriority Priority { get; set; }

        // Active?
        public bool IsActive { get; set; } = true;

    }

    // Urgency of Announcment
    public enum AnnouncementPriority
    {
        Low,
        Normal,
        High,
        Urgent
    }

}