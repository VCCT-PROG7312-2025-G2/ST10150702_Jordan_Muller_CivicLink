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
        private readonly IServiceRequestService _serviceRequestService;

        public HomeController(ILogger<HomeController> logger, IIssueService issueService, IGamificationService gamificationService, IEventService eventService, IServiceRequestService serviceRequestService)
        {
            _logger = logger;
            _issueService = issueService;
            _gamificationService = gamificationService;
            _eventService = eventService;
            _serviceRequestService = serviceRequestService;
            _serviceRequestService = serviceRequestService;
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
                    // This instant feedback is crucial for user satisfaction and the user engagement strategy as a whole
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

            // This is if something has gone wrong and we need to redisplay the form
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

        // This is a default MVC method that came with the template
        // Im not sure if we are ever expected to actually build out the privacy page but at the moment its blank
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
            // Learning about lambda expressions and LINQ has been a game changer for me
            // If I had this back in Grade 12 I would have saved myself so much time
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


        // Display service request status page with all tracking features
        public async Task<IActionResult> ServiceRequestStatus(int? searchId, string sortBy, bool priorityView = false, bool graphView = false)
        {
            var viewModel = new ServiceRequestViewModel
            {
                ShowPriorityView = priorityView,
                ShowGraphView = graphView,
                SortBy = sortBy
            };

            // If searching by ID, use BST/AVL for efficient lookup
            if (searchId.HasValue)
            {
                var request = await _serviceRequestService.GetRequestByIdAsync(searchId.Value);
                if (request != null)
                {
                    viewModel.SelectedRequest = request;
                    viewModel.RelatedRequests = await _serviceRequestService.GetRelatedRequestsAsync(searchId.Value);
                    viewModel.Relationships = await _serviceRequestService.GetRequestRelationshipsAsync(searchId.Value);
                }
                viewModel.SearchQuery = searchId.ToString();
            }

            // Get service requests based on view type
            if (priorityView)
            {
                // Use heap for priority-sorted view
                viewModel.ServiceRequests = await _serviceRequestService.GetRequestsByPriorityAsync();
            }
            else
            {
                // Use BST for regular sorted view
                viewModel.ServiceRequests = await _serviceRequestService.GetAllRequestsAsync();
            }

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(sortBy))
            {
                viewModel.ServiceRequests = sortBy switch
                {
                    "status" => viewModel.ServiceRequests.OrderBy(r => r.Status).ToList(),
                    "category" => viewModel.ServiceRequests.OrderBy(r => r.Category).ToList(),
                    "date" => viewModel.ServiceRequests.OrderByDescending(r => r.CreatedAt).ToList(),
                    "priority" => viewModel.ServiceRequests.OrderByDescending(r => r.Priority).ToList(),
                    _ => viewModel.ServiceRequests
                };
            }

            // Get MST for graph view
            if (graphView)
            {
                viewModel.MinimumSpanningTree = await _serviceRequestService.GetMinimumSpanningTreeAsync();
            }

            // Get statistics
            viewModel.Statistics = await _serviceRequestService.GetRequestStatisticsAsync();

            return View(viewModel);
        }

        // API endpoint for searching request by ID (returns JSON for AJAX)
        [HttpGet]
        public async Task<IActionResult> SearchRequestById(int id)
        {
            var request = await _serviceRequestService.GetRequestByIdAsync(id);
            if (request == null)
            {
                return Json(new { success = false, message = "Request not found" });
            }

            var related = await _serviceRequestService.GetRelatedRequestsAsync(id);

            return Json(new
            {
                success = true,
                request = new
                {
                    id = request.Id,
                    title = request.Title,
                    status = request.Status.ToString(),
                    priority = request.Priority.ToString(),
                    category = request.Category.ToString(),
                    location = request.Location,
                    createdAt = request.CreatedAt.ToString("yyyy-MM-dd"),
                    description = request.Description
                },
                relatedRequests = related.Select(r => new
                {
                    id = r.Id,
                    title = r.Title,
                    status = r.Status.ToString()
                })
            });
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