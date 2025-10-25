using BookingTicketCinema.Models;
using BookingTicketCinema.Services;
using BookingTicketCinema.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingTicketCinema.Controllers
{
    public static class IdentityUserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/signup", CreateUser);
            app.MapPost("/signin", SignIn);
            app.MapPost("/forgot-password", ForgotPassword);
            app.MapPost("/reset-password", ResetPassword);
            return app;
        }

        [AllowAnonymous]
        private static async Task<IResult> CreateUser(
            UserManager<User> userManager,
            [FromBody] UserRegistrationModel userRegistrationModel)
        {
            User user = new User()
            {
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName,
                Gender = userRegistrationModel.Gender,
                DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-userRegistrationModel.Age))
            };
            var result = await userManager.CreateAsync(
                user,
                userRegistrationModel.Password);
            if (!result.Succeeded)
            {
                // Nếu tạo thất bại (ví dụ: mật khẩu sai format), trả lỗi rõ ràng
                return Results.BadRequest(new
                {
                    message = "User creation failed.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }
            await userManager.AddToRoleAsync(user, userRegistrationModel.Role);

            if (result.Succeeded) return Results.Ok(result);
            else return Results.BadRequest(result);
        }

        [AllowAnonymous]
        private static async Task<IResult> SignIn(
            UserManager<User> userManager,
            [FromBody] LoginModel loginModel,
            IOptions<AppSettings> appSettings)
            {
                var user = await userManager.FindByEmailAsync(loginModel.Email);
                if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
                {
                var roles = await userManager.GetRolesAsync(user);
                var signInKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret));
                ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
                {
                    new Claim("userID",user.Id.ToString()),
                    new Claim("gender", user.Gender.ToString()),
                    new Claim("age", (DateTime.Now.Year - user.DOB.Year).ToString()),
                    new Claim(ClaimTypes.Role, roles.First())
                });  
                var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddDays(1),// Token hợp lệ trong 1 ngày
                    SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    return Results.Ok(new { token });
                }
                else
                    return Results.BadRequest(new { message = "Username or password is incorrect." });
            }

        [AllowAnonymous]
        private static async Task<IResult> ForgotPassword(
        UserManager<User> userManager,
        [FromBody] ForgotPasswordVm forgotPasswordVm,
        EmailService emailService)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordVm.Email);
            if (user == null)
            {
                return Results.BadRequest(new { message = "User not found." });
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://your-app-url/reset-password?token={resetToken}&email={forgotPasswordVm.Email}";

            var message = $"<p>Please reset your password by clicking <a href='{resetLink}'>here</a>.</p>";
            await emailService.SendEmailAsync(forgotPasswordVm.Email, "Password Reset", message);

            return Results.Ok(new { resetToken, message = "Password reset link has been sent to your email." });
        }

        [AllowAnonymous]
        private static async Task<IResult> ResetPassword(
        UserManager<User> userManager,
        [FromBody] ResetPasswordVm resetPasswordVm)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordVm.Email);
            if (user == null)
            {
                return Results.BadRequest(new { message = "User not found." });
            }

            var resetPassResult = await userManager.ResetPasswordAsync(user, resetPasswordVm.Token, resetPasswordVm.NewPassword);
            if (resetPassResult.Succeeded)
            {
                return Results.Ok(new { message = "Password has been reset successfully." });
            }
            else
            {
                return Results.BadRequest(new { message = "Error resetting password.", errors = resetPassResult.Errors });
            }
        }
    }
}
