using BookingTicketCinemaClient.Models;
using BookingTicketCinemaClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinemaClient.Controllers;

[Route("admin/seatgroup")]
public class SeatGroupController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<SeatGroupController> _logger;

    public SeatGroupController(ApiClient apiClient, ILogger<SeatGroupController> logger)
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
            var seatGroups = await _apiClient.GetAsync<List<SeatGroupViewModel>>("seatgroups");
            
            // Fetch rooms to display room names
            var rooms = await _apiClient.GetAsync<List<RoomViewModel>>("rooms");
            
            if (rooms != null && seatGroups != null)
            {
                var roomDict = rooms.ToDictionary(r => r.RoomId, r => r.Name);
                foreach (var seatGroup in seatGroups)
                {
                    if (roomDict.ContainsKey(seatGroup.RoomId))
                    {
                        seatGroup.RoomName = roomDict[seatGroup.RoomId];
                    }
                }
            }

            ViewBag.Rooms = rooms ?? new List<RoomViewModel>();
            return View(seatGroups ?? new List<SeatGroupViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading seat groups");
            ViewBag.ErrorMessage = "Failed to load seat groups. Please try again later.";
            ViewBag.Rooms = new List<RoomViewModel>();
            return View(new List<SeatGroupViewModel>());
        }
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromForm] CreateSeatGroupRequest request)
    {
        try
        {
            var seatGroup = await _apiClient.PostAsync<SeatGroupViewModel>("seatgroups", request);
            if (seatGroup != null)
            {
                TempData["SuccessMessage"] = "Seat Group created successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating seat group");
            TempData["ErrorMessage"] = $"Failed to create seat group: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Route("{id}/update")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateSeatGroupRequest request)
    {
        try
        {
            var seatGroup = await _apiClient.PutAsync<SeatGroupViewModel>($"seatgroups/{id}", request);
            if (seatGroup != null)
            {
                TempData["SuccessMessage"] = "Seat Group updated successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating seat group");
            TempData["ErrorMessage"] = $"Failed to update seat group: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Route("{id}/delete")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            var success = await _apiClient.DeleteAsync($"seatgroups/{id}");
            if (success)
            {
                TempData["SuccessMessage"] = "Seat Group deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete seat group.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting seat group");
            TempData["ErrorMessage"] = $"Failed to delete seat group: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}

