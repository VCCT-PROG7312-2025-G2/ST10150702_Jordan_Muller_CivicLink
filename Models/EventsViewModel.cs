namespace CivicLink.Models
{
  
    // Again, creating a model as simple as this below is second nature to me at this point
    // I found myself checking syntax for creating the HashSet, Queue and Stack properties below though
    // Would probably use AI to create this part for me next time
    public class EventsViewModel
    {
        // All events to display
        public List<Event> Events { get; set; } = new List<Event>();

        // All announcements to show
        public List<Announcement> Announcements { get; set; } = new List<Announcement>();

        // Available categories for filtering
        public HashSet<EventCategory> AvailableCategories { get; set; } = new HashSet<EventCategory>();

        // Unique dates that have events
        public HashSet<DateTime> EventDates { get; set; } = new HashSet<DateTime>();

        // Events recommended based on user searches
        public Queue<Event> RecommendedEvents { get; set; } = new Queue<Event>();

        // Recently viewed events
        public Stack<Event> RecentlyViewed { get; set; } = new Stack<Event>();

        // Current search term if any
        public string SearchTerm { get; set; }

        // Current category filter if any
        public EventCategory? FilterCategory { get; set; }

        // Current date filter if any
        public DateTime? FilterDate { get; set; }
    }
}
