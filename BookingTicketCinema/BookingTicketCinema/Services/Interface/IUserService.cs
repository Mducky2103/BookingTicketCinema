using BookingTicketCinema.Models;
using Microsoft.AspNetCore.Identity;

namespace BookingTicketCinema.Services.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<object>> GetAllUsersAsync();
        Task<object?> GetUserByIdAsync(string id);
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task<IdentityResult> AddToRoleAsync(string userId, string roleName);
        Task<IList<string>> GetUserRolesAsync(string userId);
        //update user role
        Task<IdentityResult> UpdateUserRoleAsync(string userId, string newRoleName);
    }
}
