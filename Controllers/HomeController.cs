using CivicLink.Models;
using CivicLink.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

// Main controller handling issue reporting, dashboard display, and user interaction with no business logic
namespace CivicLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIssueService _issueService;
        private readonly IGamificationService _gamificationService;

        public HomeController(ILogger<HomeController> logger, IIssueService issueService, IGamificationService gamificationService)
        {
            _logger = logger;
            _issueService = issueService;
            _gamificationService = gamificationService;
        }

        public async Task<IActionResult> Index()
        {
            // Get user engagement for display (using a default user ID for demo)
            var userEngagement = await _gamificationService.GetUserEngagementAsync("demo-user");
            ViewBag.UserEngagement = userEngagement;
            return View();
        }

        public async Task<IActionResult> ReportIssue()
        {
            var userEngagement = await _gamificationService.GetUserEngagementAsync("demo-user");
            var availableBadges = await _gamificationService.GetAvailableBadgesAsync();

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
        public async Task<IActionResult> ReportIssue(ReportIssueViewModel model)
        {
            // Only validate the Issue part of the model
            ModelState.Remove("UserEngagement");
            ModelState.Remove("AvailableBadges");
            ModelState.Remove("SuccessMessage");

            if (ModelState.IsValid && model.Issue != null)
            {
                try
                {
                    // Create the issue
                    var issueId = await _issueService.CreateIssueAsync(model.Issue);

                    // Update user engagement
                    var pointsEarned = CalculatePointsForIssue(model.Issue);
                    var updatedEngagement = await _gamificationService.UpdateUserEngagementAsync(
                        "demo-user", pointsEarned, "issue reported");

                    // Set success message
                    TempData["SuccessMessage"] = $"Issue reported successfully! Issue ID: {issueId}. You earned {pointsEarned} points!";
                    TempData["ShowBadges"] = updatedEngagement.Badges.Count > (model.UserEngagement?.Badges.Count ?? 0);

                    return RedirectToAction(nameof(ReportIssue));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating issue");
                    ModelState.AddModelError("", "An error occurred while submitting your issue. Please try again.");
                }
            }

            // If we get here, something failed, redisplay form
            model.UserEngagement = await _gamificationService.GetUserEngagementAsync("demo-user");
            model.AvailableBadges = await _gamificationService.GetAvailableBadgesAsync();
            return View(model);
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