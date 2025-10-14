// Business logic for event management with recommendation algorithm and search pattern analysis
using CivicLink.Models;
using CivicLink.DataStructures;

namespace CivicLink.Services
{
    /* 
     * Summary: Handles all business logic for events and announcements.
     * Manages event storage using SortedDictionary, tracks search patterns,
     * and generates recommendations based on user behavior.
     */


    // Interfaces use to be quite a weakness of mine however I have improved a lot in this area and used the following for help:
    // Based on Microsoft Docs: Interfaces and async/await programming patterns
    // https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces
    // https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
    public interface IEventService
    {
        Task<int> CreateEventAsync(Event ev);
        Task<Event> GetEventByIdAsync(int id);
        Task<List<Event>> GetAllEventsAsync();
        Task<List<Event>> SearchEventsAsync(string searchTerm, EventCategory? category, DateTime? date);
        Task<List<Event>> GetRecommendedEventsAsync();
        Task RecordEventViewAsync(Event ev);
        Task<List<Event>> GetRecentlyViewedAsync();
        void TrackSearch(string searchTerm, EventCategory? category, DateTime? date);
        Task<HashSet<EventCategory>> GetActiveCategoriesAsync();
        Task<HashSet<DateTime>> GetEventDatesAsync();
    }

    public class EventService : IEventService
    {
        // Main storage for events
        private readonly EventSortedDictionary eventStorage;

        // Tracks what users search for
        private readonly SearchPatternTracker searchTracker;

        // Recently viewed events
        private readonly RecentlyViewedStack recentlyViewed;

        // Active categories
        private readonly CategoryManager categoryManager;

        // Counter for new event IDs
        private static int nextId = 1;

        public EventService()
        {
            eventStorage = new EventSortedDictionary();
            searchTracker = new SearchPatternTracker();
            recentlyViewed = new RecentlyViewedStack();
            categoryManager = new CategoryManager();

            // Add sample events when service starts
            InitializeSampleEvents();
        }

        // Create a new event in the system
        public async Task<int> CreateEventAsync(Event ev)
        {
            return await Task.Run(() =>
            {
                ev.Id = nextId++;
                ev.CreatedAt = DateTime.Now;
                ev.IsActive = true;

                eventStorage.Add(ev);
                categoryManager.Add(ev.Category);

                return ev.Id;
            });
        }

        // Get a specific event
        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await Task.Run(() => eventStorage.GetById(id));
        }

        // Get all events sorted by date
        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await Task.Run(() =>
            {
                return eventStorage.GetSortedByDate()
                    .Where(e => e.IsActive)
                    .ToList();
            });
        }

        // Based on Microsoft Docs: Asynchronous programming in C#
        // and LINQ filtering best practices.
        // https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/
        // Search events with filters
        public async Task<List<Event>> SearchEventsAsync(string searchTerm, EventCategory? category, DateTime? date)
        {
            return await Task.Run(() =>
            {
                var events = eventStorage.GetAll().Where(e => e.IsActive);

                // Filter by search term if provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    events = events.Where(e =>
                        e.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                }

                // Filter by category if provided
                if (category.HasValue)
                {
                    events = events.Where(e => e.Category == category.Value);
                }

                // Filter by date if provided
                if (date.HasValue)
                {
                    events = events.Where(e =>
                        e.StartDate.Date == date.Value.Date ||
                        (e.EndDate.HasValue && e.EndDate.Value.Date == date.Value.Date));
                }

                return events.OrderBy(e => e.StartDate).ToList();
            });
        }

        // Track user search to build recommendations
        public void TrackSearch(string searchTerm, EventCategory? category, DateTime? date)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTracker.RecordSearchTerm(searchTerm);
            }

            if (category.HasValue)
            {
                searchTracker.RecordCategorySearch(category.Value);
            }

            if (date.HasValue)
            {
                searchTracker.RecordDateSearch(date.Value);
            }
        }

        // Get recommended events based on user search patterns
        public async Task<List<Event>> GetRecommendedEventsAsync()
        {
            return await Task.Run(() =>
            {
                var recommendations = new List<Event>();

                // Get categories user searches most
                var topCategories = searchTracker.GetTopCategories(3);

                // Find upcoming events in those categories
                foreach (var category in topCategories)
                {
                    var categoryEvents = eventStorage.GetByCategory(category)
                        .Where(e => e.IsActive && e.StartDate >= DateTime.Now)
                        .OrderBy(e => e.StartDate)
                        .Take(2);

                    recommendations.AddRange(categoryEvents);
                }

                // Remove duplicates and limit to 6 recommendations
                return recommendations
                    .GroupBy(e => e.Id)
                    .Select(g => g.First())
                    .Take(6)
                    .ToList();
            });
        }

        // Record when user views an event
        public async Task RecordEventViewAsync(Event ev)
        {
            await Task.Run(() =>
            {
                if (ev != null)
                {
                    recentlyViewed.Push(ev);
                }
            });
        }

        // Get events user recently viewed
        public async Task<List<Event>> GetRecentlyViewedAsync()
        {
            return await Task.Run(() => recentlyViewed.GetRecent(5));
        }

        // Get all active categories
        public async Task<HashSet<EventCategory>> GetActiveCategoriesAsync()
        {
            return await Task.Run(() => categoryManager.GetAll());
        }

        // Get unique dates that have events
        public async Task<HashSet<DateTime>> GetEventDatesAsync()
        {
            return await Task.Run(() =>
            {
                var dates = new HashSet<DateTime>();
                var events = eventStorage.GetAll();

                foreach (var ev in events.Where(e => e.IsActive))
                {
                    dates.Add(ev.StartDate.Date);
                    if (ev.EndDate.HasValue)
                    {
                        dates.Add(ev.EndDate.Value.Date);
                    }
                }

                return dates;
            });
        }

        // Added sample data for testing
        // The rubric said to add at least 15 sample events with diverse categories, dates, locations, and details.
        /* 
         * ============================================
         * Generated by Claude 2025-01-15
         * ============================================
         */
        // I chose to have Claude generate the sample events for me as it would take me a long time to think of 15 different events with all the details.
        private void InitializeSampleEvents()
        {
            var sampleEvents = new[]
            {
                new Event
                {
                    Id = nextId++,
                    Name = "Cape Town Community Town Hall Meeting",
                    Description = "Join us for a community town hall meeting to discuss upcoming infrastructure projects, budget allocations, and answer questions from residents about municipal services.",
                    Category = EventCategory.TownHall,
                    Location = "Cape Town Civic Centre, 12 Hertzog Boulevard",
                    StartDate = DateTime.Now.AddDays(5),
                    EndDate = DateTime.Now.AddDays(5).AddHours(3),
                    ContactPerson = "Sarah Mbeki",
                    ContactPhone = "021-400-1234",
                    ContactEmail = "townhall@capetown.gov.za",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Table Mountain Hiking Safety Workshop",
                    Description = "Learn essential hiking safety tips, emergency procedures, and mountain weather awareness. Perfect for both beginners and experienced hikers exploring Table Mountain trails.",
                    Category = EventCategory.Safety,
                    Location = "Table Mountain National Park Visitor Centre",
                    StartDate = DateTime.Now.AddDays(8),
                    EndDate = DateTime.Now.AddDays(8).AddHours(2),
                    ContactPerson = "John van der Merwe",
                    ContactPhone = "021-712-0527",
                    ContactEmail = "safety@tmnp.co.za",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-8),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Waterfront Jazz Festival",
                    Description = "Experience the best of Cape Town's jazz scene with local and international artists performing at the V&A Waterfront. Food stalls and craft markets available.",
                    Category = EventCategory.Festival,
                    Location = "V&A Waterfront Amphitheatre",
                    StartDate = DateTime.Now.AddDays(15),
                    EndDate = DateTime.Now.AddDays(17),
                    ContactPerson = "Linda Jacobs",
                    ContactPhone = "021-408-7600",
                    ContactEmail = "events@waterfront.co.za",
                    IsFree = false,
                    Cost = 150.00m,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Khayelitsha Youth Sports Day",
                    Description = "Annual sports day for youth featuring soccer, netball, athletics, and more. All young people welcome to participate or spectate. Refreshments provided.",
                    Category = EventCategory.Sports,
                    Location = "Khayelitsha Sports Complex",
                    StartDate = DateTime.Now.AddDays(12),
                    EndDate = DateTime.Now.AddDays(12).AddHours(6),
                    ContactPerson = "Thabo Ndlovu",
                    ContactPhone = "021-360-1234",
                    ContactEmail = "sports@khayelitsha.org.za",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-12),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Environmental Conservation Workshop",
                    Description = "Learn about local conservation efforts, water saving techniques, and how to reduce your environmental footprint. Suitable for all ages.",
                    Category = EventCategory.Environment,
                    Location = "Green Point Urban Park",
                    StartDate = DateTime.Now.AddDays(20),
                    EndDate = DateTime.Now.AddDays(20).AddHours(3),
                    ContactPerson = "Dr. Emma Green",
                    ContactPhone = "021-487-2100",
                    ContactEmail = "conservation@capetown.gov.za",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Artisan Market at Oranjezicht",
                    Description = "Browse handmade crafts, organic produce, baked goods, and local art from Cape Town's talented artisans. Live music and food trucks available.",
                    Category = EventCategory.Arts,
                    Location = "Oranjezicht City Farm",
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(3).AddHours(5),
                    ContactPerson = "Maria du Plessis",
                    ContactPhone = "021-447-8920",
                    ContactEmail = "market@ozcf.co.za",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-7),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Public Health & Wellness Fair",
                    Description = "Free health screenings, wellness information, vaccination services, and health education. Medical professionals available for consultations.",
                    Category = EventCategory.Health,
                    Location = "Cape Town International Convention Centre",
                    StartDate = DateTime.Now.AddDays(18),
                    EndDate = DateTime.Now.AddDays(18).AddHours(6),
                    ContactPerson = "Dr. Zanele Khumalo",
                    ContactPhone = "021-938-4000",
                    ContactEmail = "health@capetown.gov.za",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-14),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Digital Skills Training for Seniors",
                    Description = "Free workshop teaching smartphone basics, internet safety, online banking, and social media to senior citizens. Bring your device or use provided tablets.",
                    Category = EventCategory.Education,
                    Location = "Cape Town Central Library",
                    StartDate = DateTime.Now.AddDays(10),
                    EndDate = DateTime.Now.AddDays(10).AddHours(2),
                    ContactPerson = "Peter Williams",
                    ContactPhone = "021-400-3000",
                    ContactEmail = "library@capetown.gov.za",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-6),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Neighbourhoodwatch Training Session",
                    Description = "Join your local neighbourhood watch. Learn community policing basics, emergency response, and how to coordinate with SAPS effectively.",
                    Category = EventCategory.Safety,
                    Location = "Wynberg Police Station Community Hall",
                    StartDate = DateTime.Now.AddDays(7),
                    EndDate = DateTime.Now.AddDays(7).AddHours(2),
                    ContactPerson = "Captain James Smith",
                    ContactPhone = "021-710-7400",
                    ContactEmail = "wynberg@saps.gov.za",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-9),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Heritage Day Celebration at Castle",
                    Description = "Celebrate South Africa's diverse heritage with traditional music, dance performances, food stalls representing all cultures, and historical tours.",
                    Category = EventCategory.Festival,
                    Location = "Castle of Good Hope",
                    StartDate = DateTime.Now.AddDays(25),
                    EndDate = DateTime.Now.AddDays(25).AddHours(8),
                    ContactPerson = "Nomsa Dlamini",
                    ContactPhone = "021-787-1260",
                    ContactEmail = "heritage@castle.co.za",
                    IsFree = false,
                    Cost = 50.00m,
                    CreatedAt = DateTime.Now.AddDays(-20),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Small Business Development Workshop",
                    Description = "Learn business registration processes, tax requirements, accessing funding, and marketing strategies for small entrepreneurs and startups.",
                    Category = EventCategory.Workshop,
                    Location = "Cape Town Chamber of Commerce",
                    StartDate = DateTime.Now.AddDays(14),
                    EndDate = DateTime.Now.AddDays(14).AddHours(4),
                    ContactPerson = "Michael Chen",
                    ContactPhone = "021-402-4300",
                    ContactEmail = "business@capechamber.co.za",
                    IsFree = false,
                    Cost = 200.00m,
                    CreatedAt = DateTime.Now.AddDays(-11),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Beach Cleanup - Muizenberg",
                    Description = "Join fellow environmentally conscious citizens for a beach cleanup. Bags and gloves provided. Refreshments after cleanup.",
                    Category = EventCategory.Environment,
                    Location = "Muizenberg Beach Main Pavilion",
                    StartDate = DateTime.Now.AddDays(6),
                    EndDate = DateTime.Now.AddDays(6).AddHours(3),
                    ContactPerson = "Lisa Thompson",
                    ContactPhone = "021-788-5400",
                    ContactEmail = "cleanup@beaches.co.za",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-4),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Youth Leadership Summit",
                    Description = "Empowering young leaders through workshops on public speaking, community organizing, and civic engagement. Ages 16-25 welcome.",
                    Category = EventCategory.Education,
                    Location = "University of Cape Town Graduate School of Business",
                    StartDate = DateTime.Now.AddDays(22),
                    EndDate = DateTime.Now.AddDays(24),
                    ContactPerson = "Ayanda Mthembu",
                    ContactPhone = "021-406-1111",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-13),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Water Crisis Information Session",
                    Description = "Important meeting regarding water conservation measures, restrictions, and drought management strategies. Q&A with city officials.",
                    Category = EventCategory.CommunityMeeting,
                    Location = "Bellville Civic Centre",
                    StartDate = DateTime.Now.AddDays(4),
                    EndDate = DateTime.Now.AddDays(4).AddHours(2),
                    ContactPerson = "David Botha",
                    ContactPhone = "021-959-1111",
                    ContactEmail = "water@capetown.gov.za",
                    IsFree = true,
                    CreatedAt = DateTime.Now.AddDays(-3),
                    IsActive = true
                },
                new Event
                {
                    Id = nextId++,
                    Name = "Summer Concert Series Finale",
                    Description = "Final concert of the summer series featuring Cape Town Philharmonic Orchestra. Bring picnic blankets and enjoy music under the stars.",
                    Category = EventCategory.Arts,
                    Location = "Kirstenbosch National Botanical Garden",
                    StartDate = DateTime.Now.AddDays(30),
                    EndDate = DateTime.Now.AddDays(30).AddHours(3),
                    ContactPerson = "Catherine Stewart",
                    ContactPhone = "021-799-8783",
                    ContactEmail = "concerts@sanbi.org.za",
                    IsFree = false,
                    Cost = 180.00m,
                    CreatedAt = DateTime.Now.AddDays(-18),
                    IsActive = true
                }
            };

            // Add all sample events to storage
            foreach (var ev in sampleEvents)
            {
                eventStorage.Add(ev);
                categoryManager.Add(ev.Category);
            }
        }
    }
}