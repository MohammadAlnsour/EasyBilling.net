using BillingSystem.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BillingSystem.API.ActionFilters
{
    public class IdempotencyFilter : ActionFilterAttribute
    {
        private readonly IIdempotentKeyService _idempotentKeyService;

        public IdempotencyFilter(IIdempotentKeyService idempotentKeyService)
        {
            _idempotentKeyService = idempotentKeyService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var key = context.HttpContext.Request.Headers["x-idempotent-key"].FirstOrDefault();

            if (string.IsNullOrEmpty(key))
            {
                context.Result = new BadRequestObjectResult("Idempotent-Key header is required.");
                return;
            }

            if (_idempotentKeyService.ExistsAsync(key).Result)
            {
                context.Result = new ConflictObjectResult("Duplicate request.");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
