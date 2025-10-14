using CivicLink.Models;
using CivicLink.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


/*
 * I use to include almost all my business logic to my MVC home controller, which I now know is bad practice
 * I still found myself doing that here, but I have moved a lot of the logic to the services layer now
 * Im still not 100% sure if this is the best way to do it, but its better than before
*/
namespace CivicLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIssueService _issueService;
        private readonly IGamificationService _gamificationService;
        private readonly IEventService _eventService;

        public HomeController(ILogger<HomeController> logger, IIssueService issueService, IGamificationService gamificationService, IEventService eventService)
        {
            _logger = logger;
            _issueService = issueService;
            _gamificationService = gamificationService;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            // Get user engagement for display (using a default user ID for demo)
            var userEngagement = await _gamificationService.GetUserEngagementAsync("demo-user");
            ViewBag.UserEngagement = userEngagement;
            return View();
        }

        // This is the method where the bug was, which was stopping me from accessing the ReportIssue page
        public async Task<IActionResult> ReportIssue()
        {
            var userEngagement = await _gamificationService.GetUserEngagementAsync("demo-user");
            var availableBadges = await _gamificationService.GetAvailableBadgesAsync();

            // Create the view model with Issue and engagement data
            var viewModel = new ReportIssueViewModel
            {
                Issue = new Issue { Priority = IssuePriority.Medium },
                UserEngagement = userEngagement,
                AvailableBadges = availableBadges
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportIssue(ReportIssueViewModel viewModel)
        {
            if (ModelState.IsValid && viewModel?.Issue != null)
            {
                try
                {
                    // Create the issue
                    var issueId = await _issueService.CreateIssueAsync(viewModel.Issue);

                    // Update user engagement
                    var pointsEarned = CalculatePointsForIssue(viewModel.Issue);
                    var currentEngagement = await _gamificationService.GetUserEngagementAsync("demo-user");
                    var updatedEngagement = await _gamificationService.UpdateUserEngagementAsync(
                        "demo-user", pointsEarned, "issue reported");

                    // Set success message
                    TempData["SuccessMessage"] = $"Issue reported successfully! Issue ID: {issueId}. You earned {pointsEarned} points!";
                    TempData["ShowBadges"] = updatedEngagement.Badges.Count > currentEngagement.Badges.Count;

                    return RedirectToAction(nameof(ReportIssue));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating issue");
                    ModelState.AddModelError("", "An error occurred while submitting your issue. Please try again.");
                }
            }

            // If we get here, something failed, redisplay form
            ViewBag.UserEngagement = await _gamificationService.GetUserEngagementAsync("demo-user");
            ViewBag.AvailableBadges = await _gamificationService.GetAvailableBadgesAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> IssuesList()
        {
            var issues = await _issueService.GetAllIssuesAsync();
            return View(issues);
        }

        public async Task<IActionResult> IssueDetails(int id)
        {
            var issue = await _issueService.GetIssueByIdAsync(id);
            if (issue == null)
            {
                return NotFound();
            }
            return View(issue);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Display events and announcements page
        public async Task<IActionResult> Events(string searchTerm, EventCategory? category, DateTime? date, string sortBy)
        {
            // Track the search for recommendations
            if (!string.IsNullOrWhiteSpace(searchTerm) || category.HasValue || date.HasValue)
            {
                _eventService.TrackSearch(searchTerm, category, date);
            }

            // Get filtered events
            var events = await _eventService.SearchEventsAsync(searchTerm, category, date);

            // Apply sorting
            events = sortBy switch
            {
                "name" => events.OrderBy(e => e.Name).ToList(),
                "category" => events.OrderBy(e => e.Category).ToList(),
                "date" => events.OrderBy(e => e.StartDate).ToList(),
                _ => events.OrderBy(e => e.StartDate).ToList()
            };

            // Build view model
            var viewModel = new EventsViewModel
            {
                Events = events,
                AvailableCategories = await _eventService.GetActiveCategoriesAsync(),
                EventDates = await _eventService.GetEventDatesAsync(),
                RecommendedEvents = new Queue<Event>(await _eventService.GetRecommendedEventsAsync()),
                SearchTerm = searchTerm,
                FilterCategory = category,
                FilterDate = date
            };

            return View(viewModel);
        }

        // View single event details
        public async Task<IActionResult> EventDetails(int id)
        {
            var ev = await _eventService.GetEventByIdAsync(id);

            if (ev == null)
            {
                return NotFound();
            }

            // Record that user viewed this event
            await _eventService.RecordEventViewAsync(ev);

            return View(ev);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private int CalculatePointsForIssue(Issue issue)
        {
            int basePoints = 20;
            int priorityMultiplier = (int)issue.Priority;
            int categoryBonus = issue.Category switch
            {
                IssueCategory.PublicSafety => 15,
                IssueCategory.WaterAndSanitation => 10,
                IssueCategory.ElectricityAndPower => 10,
                _ => 5
            };

            return basePoints + (priorityMultiplier * 5) + categoryBonus;
        }
    }
}