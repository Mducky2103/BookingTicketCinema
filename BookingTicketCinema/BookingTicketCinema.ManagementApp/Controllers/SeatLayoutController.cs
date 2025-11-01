using BookingTicketCinema.ManagementApp.Models;
using BookingTicketCinema.ManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.ManagementApp.Controllers;

[Route("admin/seatlayout")]
public class SeatLayoutController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<SeatLayoutController> _logger;

    public SeatLayoutController(ApiClient apiClient, ILogger<SeatLayoutController> logger)
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
            var seats = await _apiClient.GetAsync<List<SeatViewModel>>("seats");
            
            // Fetch rooms and seat groups to display names
            var rooms = await _apiClient.GetAsync<List<RoomViewModel>>("rooms");
            var seatGroups = await _apiClient.GetAsync<List<SeatGroupViewModel>>("seatgroups");
            
            if (seats != null)
            {
                var roomDict = rooms?.ToDictionary(r => r.RoomId, r => r.Name) ?? new Dictionary<int, string>();
                var seatGroupDict = seatGroups?.ToDictionary(sg => sg.SeatGroupId, sg => sg.GroupName) ?? new Dictionary<int, string>();
                
                foreach (var seat in seats)
                {
                    if (roomDict.ContainsKey(seat.RoomId))
                    {
                        seat.RoomName = roomDict[seat.RoomId];
                    }
                    if (seatGroupDict.ContainsKey(seat.SeatGroupId))
                    {
                        seat.SeatGroupName = seatGroupDict[seat.SeatGroupId];
                    }
                }
            }

            ViewBag.Rooms = rooms ?? new List<RoomViewModel>();
            ViewBag.SeatGroups = seatGroups ?? new List<SeatGroupViewModel>();
            return View(seats ?? new List<SeatViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading seats");
            ViewBag.ErrorMessage = "Failed to load seats. Please try again later.";
            ViewBag.Rooms = new List<RoomViewModel>();
            ViewBag.SeatGroups = new List<SeatGroupViewModel>();
            return View(new List<SeatViewModel>());
        }
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromForm] CreateSeatRequest request)
    {
        try
        {
            var seat = await _apiClient.PostAsync<SeatViewModel>("seats", request);
            if (seat != null)
            {
                TempData["SuccessMessage"] = "Seat created successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating seat");
            TempData["ErrorMessage"] = $"Failed to create seat: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Route("{id}/update")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateSeatRequest request)
    {
        try
        {
            var seat = await _apiClient.PutAsync<SeatViewModel>($"seats/{id}", request);
            if (seat != null)
            {
                TempData["SuccessMessage"] = "Seat updated successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating seat");
            TempData["ErrorMessage"] = $"Failed to update seat: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Route("{id}/delete")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            var success = await _apiClient.DeleteAsync($"seats/{id}");
            if (success)
            {
                TempData["SuccessMessage"] = "Seat deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete seat.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting seat");
            TempData["ErrorMessage"] = $"Failed to delete seat: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Route("get-seatgroups-by-room/{roomId}")]
    public async Task<IActionResult> GetSeatGroupsByRoom(int roomId)
    {
        try
        {
            var seatGroups = await _apiClient.GetAsync<List<SeatGroupViewModel>>($"seatgroups/room/{roomId}");
            return Json(seatGroups ?? new List<SeatGroupViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading seat groups for room {RoomId}", roomId);
            return Json(new List<SeatGroupViewModel>());
        }
    }
}

