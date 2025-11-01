using BookingTicketCinema.DTO;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.Controllers
{
    public static class PriceRuleEndpoints
    {
        public static IEndpointRouteBuilder MapPriceRuleEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/pricerules", GetAllPriceRules);
            app.MapGet("/pricerules/{id}", GetPriceRuleById);
            app.MapGet("/pricerules/seatgroup/{seatGroupId}", GetPriceRulesBySeatGroup);
            app.MapPost("/pricerules", CreatePriceRule);
            app.MapPut("/pricerules/{id}", UpdatePriceRule);
            app.MapDelete("/pricerules/{id}", DeletePriceRule);
            return app;
        }
        [AllowAnonymous]
        private static async Task<IResult> GetAllPriceRules(IPriceRuleService priceRuleService)
        {
            var priceRules = await priceRuleService.GetAllAsync();
            return Results.Ok(priceRules);
        }
        [AllowAnonymous]
        private static async Task<IResult> GetPriceRuleById(int id, IPriceRuleService priceRuleService)
        {
            var priceRule = await priceRuleService.GetByIdAsync(id);
            if (priceRule == null) return Results.NotFound(new { message = "PriceRule not found" });
            return Results.Ok(priceRule);
        }
        [AllowAnonymous]
        private static async Task<IResult> GetPriceRulesBySeatGroup(int seatGroupId, IPriceRuleService priceRuleService)
        {
            var priceRules = await priceRuleService.GetBySeatGroupIdAsync(seatGroupId);
            return Results.Ok(priceRules);
        }
        [AllowAnonymous]
        private static async Task<IResult> CreatePriceRule([FromBody] CreatePriceRuleDto dto, IPriceRuleService priceRuleService)
        {
            try
            {
                var priceRule = await priceRuleService.CreateAsync(dto);
                return Results.Created($"/pricerules/{priceRule.PriceRuleId}", priceRule);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }
        [AllowAnonymous]
        private static async Task<IResult> UpdatePriceRule(int id, [FromBody] UpdatePriceRuleDto dto, IPriceRuleService priceRuleService)
        {
            try
            {
                var priceRule = await priceRuleService.UpdateAsync(id, dto);
                if (priceRule == null) return Results.NotFound(new { message = "PriceRule not found" });
                return Results.Ok(priceRule);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }
        [AllowAnonymous]
        private static async Task<IResult> DeletePriceRule(int id, IPriceRuleService priceRuleService)
        {
            var result = await priceRuleService.DeleteAsync(id);
            if (!result) return Results.NotFound(new { message = "PriceRule not found" });
            return Results.Ok(new { message = "PriceRule deleted successfully" });
        }
    }
}


