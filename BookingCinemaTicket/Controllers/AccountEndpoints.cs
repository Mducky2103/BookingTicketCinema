using BookingCinemaTicket.Models;
using BookingCinemaTicket.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingCinemaTicket.Controllers
{
    public static class AccountEndpoints
    {
        public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/UserProfile", GetUserProfile);
            app.MapPut("/EditUserProfile", EditUserProfile);
            return app;
        }

        [Authorize]
        private static async Task<IResult> GetUserProfile(
            ClaimsPrincipal user,
            UserManager<User> userManager)
        {
            string userID = user.Claims.First(x => x.Type == "userID").Value;
            var userDetails = await userManager.FindByIdAsync(userID);
            return Results.Ok(
                new
                {
                    Email = userDetails?.Email,
                    FullName = userDetails?.FullName,
                    Gender = userDetails?.Gender,
                    DateOfBirth = userDetails?.DOB
                });
        }

        [Authorize]
        private static async Task<IResult> EditUserProfile(
            ClaimsPrincipal user,
            UserManager<User> userManager,
            [FromBody] EditProfileVm editProfileVm)
        {
            string userID = user.Claims.First(x => x.Type == "userID").Value;
            var userDetails = await userManager.FindByIdAsync(userID);

            if (userDetails == null)
            {
                return Results.NotFound(new { message = "User not found." });
            }
            bool isUpdated = false;

            if (!string.IsNullOrWhiteSpace(editProfileVm.FullName) && editProfileVm.FullName != userDetails.FullName)
            {
                userDetails.FullName = editProfileVm.FullName;
                isUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(editProfileVm.Gender) && editProfileVm.Gender != userDetails.Gender)
            {
                userDetails.Gender = editProfileVm.Gender;
                isUpdated = true;
            }

            if (editProfileVm.DateOfBirth.HasValue && DateOnly.FromDateTime(editProfileVm.DateOfBirth.Value) != userDetails.DOB)
            {
                userDetails.DOB = DateOnly.FromDateTime(editProfileVm.DateOfBirth.Value);
                isUpdated = true;
            }

            if (!isUpdated)
            {
                return Results.Ok(new { message = "No changes detected." });
            }

            var result = await userManager.UpdateAsync(userDetails);

            if (result.Succeeded)
            {
                return Results.Ok(new { message = "Profile updated successfully." });
            }
            else
            {
                return Results.BadRequest(new { message = "Error updating profile.", errors = result.Errors });
            }
        }
    }
}
