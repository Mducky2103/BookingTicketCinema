using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace BookingTicketCinema.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepo;

        public UserService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserRepository userRepo)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepo = userRepo;
        }

        //public async Task<IEnumerable<User>> GetAllUsersAsync()
        //    => await _userRepo.GetAllAsync();
        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();

            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Gender,
                    user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "N/A"
                });
            }

            return result;
        }

        //public async Task<User?> GetUserByIdAsync(string id)
        //    => await _userRepo.GetByIdAsync(id);
        public async Task<object?> GetUserByIdAsync(string id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Gender,
                user.PhoneNumber,
                user.DOB,
                Role = roles.FirstOrDefault() ?? "N/A"
            };
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            var existing = await _userManager.FindByIdAsync(user.Id);
            if (existing == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            existing.Email = user.Email;
            existing.FullName = user.FullName;
            existing.PhoneNumber = user.PhoneNumber;

            return await _userManager.UpdateAsync(existing);
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> AddToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();
            return await _userManager.GetRolesAsync(user);
        }
        // update user role (restrict to AllowedRoles, remove other roles, add new role)
        public async Task<IdentityResult> UpdateUserRoleAsync(string userId, string newRoleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (string.IsNullOrWhiteSpace(newRoleName) || !AllowedRoles.Contains(newRoleName))
                return IdentityResult.Failed(new IdentityError { Description = "Role is not allowed. Allowed roles: Admin, Staff, Customer" });

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Ensure the new role exists (only create if allowed)
            if (!await _roleManager.RoleExistsAsync(newRoleName))
            {
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(newRoleName));
                if (!createRoleResult.Succeeded)
                    return createRoleResult;
            }

            // Remove from any roles that are not the new role
            var rolesToRemove = currentRoles
                .Where(r => !string.Equals(r, newRoleName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                    return removeResult;
            }

            // Add to new role if not already in it
            if (!currentRoles.Any(r => string.Equals(r, newRoleName, StringComparison.OrdinalIgnoreCase)))
            {
                var addResult = await _userManager.AddToRoleAsync(user, newRoleName);
                if (!addResult.Succeeded)
                    return addResult;
            }

            return IdentityResult.Success;
        }
        // Only these roles are allowed
        private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            "Admin",
            "Staff",
            "Customer"
        };

    }
}
