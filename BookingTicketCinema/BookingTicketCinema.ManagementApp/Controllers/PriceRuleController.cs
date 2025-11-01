using BookingTicketCinema.ManagementApp.Models;
using BookingTicketCinema.ManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.ManagementApp.Controllers;

[Route("admin/pricerule")]
public class PriceRuleController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<PriceRuleController> _logger;

    public PriceRuleController(ApiClient apiClient, ILogger<PriceRuleController> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var priceRules = await _apiClient.GetAsync<List<PriceRuleViewModel>>("pricerules");
            
            // Fetch seat groups to display names
            var seatGroups = await _apiClient.GetAsync<List<SeatGroupViewModel>>("seatgroups");
            
            if (priceRules != null && seatGroups != null)
            {
                var seatGroupDict = seatGroups.ToDictionary(sg => sg.SeatGroupId, sg => sg.GroupName);
                foreach (var priceRule in priceRules)
                {
                    if (seatGroupDict.ContainsKey(priceRule.SeatGroupId))
                    {
                        priceRule.SeatGroupName = seatGroupDict[priceRule.SeatGroupId];
                    }
                }
            }

            ViewBag.SeatGroups = seatGroups ?? new List<SeatGroupViewModel>();
            return View(priceRules ?? new List<PriceRuleViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading price rules");
            ViewBag.ErrorMessage = "Failed to load price rules. Please try again later.";
            ViewBag.SeatGroups = new List<SeatGroupViewModel>();
            return View(new List<PriceRuleViewModel>());
        }
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromForm] CreatePriceRuleRequest request)
    {
        try
        {
            var priceRule = await _apiClient.PostAsync<PriceRuleViewModel>("pricerules", request);
            if (priceRule != null)
            {
                TempData["SuccessMessage"] = "Price Rule created successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating price rule");
            TempData["ErrorMessage"] = $"Failed to create price rule: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Route("{id}/update")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdatePriceRuleRequest request)
    {
        try
        {
            var priceRule = await _apiClient.PutAsync<PriceRuleViewModel>($"pricerules/{id}", request);
            if (priceRule != null)
            {
                TempData["SuccessMessage"] = "Price Rule updated successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating price rule");
            TempData["ErrorMessage"] = $"Failed to update price rule: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Route("{id}/delete")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            var success = await _apiClient.DeleteAsync($"pricerules/{id}");
            if (success)
            {
                TempData["SuccessMessage"] = "Price Rule deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete price rule.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting price rule");
            TempData["ErrorMessage"] = $"Failed to delete price rule: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}

