using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userService.CreateUserAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
        {
            var user = new User
            {
                Id = id,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber   
            };

            var result = await _userService.UpdateUserAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User deleted successfully");
        }

        [HttpPost("{id}/roles")]
        public async Task<IActionResult> AddToRole(string id, [FromBody] string roleName)
        {
            var result = await _userService.AddToRoleAsync(id, roleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"User added to role {roleName}");
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            var roles = await _userService.GetUserRolesAsync(id);
            return Ok(roles);
        }
        //update user role
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] string newRoleName)
        {
            var result = await _userService.UpdateUserRoleAsync(id, newRoleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok($"User role updated to {newRoleName}");
        }
    }
}
