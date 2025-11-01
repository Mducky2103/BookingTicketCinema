using BookingTicketCinema.ManagementApp.Models;
using BookingTicketCinema.ManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.ManagementApp.Controllers;

[Route("admin/rooms")]
public class RoomsController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(ApiClient apiClient, ILogger<RoomsController> logger)
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
            var rooms = await _apiClient.GetAsync<List<RoomViewModel>>("rooms");
            return View(rooms ?? new List<RoomViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading rooms");
            ViewBag.ErrorMessage = "Failed to load rooms. Please try again later.";
            return View(new List<RoomViewModel>());
        }
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromForm] CreateRoomRequest request)
    {
        try
        {
            var room = await _apiClient.PostAsync<RoomViewModel>("rooms", request);
            if (room != null)
            {
                TempData["SuccessMessage"] = "Room created successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating room");
            TempData["ErrorMessage"] = $"Failed to create room: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Route("{id}/update")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateRoomRequest request)
    {
        try
        {
            var room = await _apiClient.PutAsync<RoomViewModel>($"rooms/{id}", request);
            if (room != null)
            {
                TempData["SuccessMessage"] = "Room updated successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating room");
            TempData["ErrorMessage"] = $"Failed to update room: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Route("{id}/delete")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            var success = await _apiClient.DeleteAsync($"rooms/{id}");
            if (success)
            {
                TempData["SuccessMessage"] = "Room deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete room.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting room");
            TempData["ErrorMessage"] = $"Failed to delete room: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}

