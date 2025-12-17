using BillingSystem.API.ActionFilters;
using BillingSystem.Application.Requests;
using BillingSystem.Application.Responses;
using BillingSystem.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InvoicesController : ControllerBase
    {
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateInvoiceRequest> _createInvoiceValidator;
        private readonly IValidator<UpdateInvoiceRequest> _updateInvoiceValidator;

        public InvoicesController(
            Serilog.ILogger logger,
            IConfiguration configuration,
            IMediator mediator,
            IValidator<CreateInvoiceRequest> createInvoiceValidator,
            IValidator<UpdateInvoiceRequest> updateInvoiceValidator)
        {
            _logger = logger;
            _configuration = configuration;
            _mediator = mediator;
            _createInvoiceValidator = createInvoiceValidator;
            _updateInvoiceValidator = updateInvoiceValidator;
        }

        //[TypeFilter(typeof(AuditBillingRequestsFilter))]
        //[Authorize]
        [TypeFilter(typeof(IdempotencyFilter))]
        [HttpPost]
        [Route("Post")]
        public async Task<IActionResult> Post(CreateInvoiceRequest invoiceRequest)
        {
            if (invoiceRequest == null)
                return BadRequest(ApiResponse<long>.IsFailed("validation error", "400"));

            var correlationId = Request.Headers["x-Correlation-Id"].FirstOrDefault();
            invoiceRequest.CorrelationId = correlationId;
            var validationResult = _createInvoiceValidator.Validate(invoiceRequest);
            if (!validationResult.IsValid)
            {
                var validationErrors = validationResult.Errors.Select(e => new ValidationError() { Code = e.ErrorCode, Message = e.ErrorMessage });
                _logger.Warning($"validation errors {validationErrors}");
                return BadRequest(ApiResponse<long>.IsFailed("validation error", "400", validationErrors));
            }

            try
            {
                var response = await _mediator.Send(invoiceRequest);
                return Ok(ApiResponse<long>.IsSuccess(response, "invoice created.", "200"));
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                _logger.Error($"exception in Create Invoice API : {ex.Message} , {ex.StackTrace}");
                return Problem("An error occured on the server, please contact system administrator.");
            }
        }

        //[TypeFilter(typeof(AuditBillingRequestsFilter))]
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(UpdateInvoiceRequest invoiceRequest)
        {
            if (invoiceRequest == null)
            {
                return BadRequest(ApiResponse<bool>.IsFailed("validation error", "400"));
            }
            var validationResult = _updateInvoiceValidator.Validate(invoiceRequest);
            if (!validationResult.IsValid)
            {
                var validationErrors = validationResult.Errors.Select(e => new ValidationError() { Code = e.ErrorCode, Message = e.ErrorMessage });
                return BadRequest(ApiResponse<bool>.IsFailed("validation error", "400", validationErrors));
            }
            try
            {
                var updated = await _mediator.Send(invoiceRequest);
                if (!updated)
                    return BadRequest(ApiResponse<bool>.IsFailed("Can't update payed or partially paid invoice.", "400"));

                return Ok(ApiResponse<bool>.IsSuccess(updated, "Invoice updated.", "200"));
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                return Problem("An error occured on the server, please contact system administrator.");
            }
        }

        [HttpGet]
        [Route("GetByTenant")]
        //[TypeFilter(typeof(AuditBillingRequestsFilter))]
        public async Task<IActionResult> GetByTenantId(int tenantId)
        {
            if (tenantId < 0) { return BadRequest("tenant Id not found"); }

            try
            {
                var request = new GetInvoicesByTenantRequest() { TenantId = tenantId };
                var response = await _mediator.Send(request);
                return Ok(ApiResponse<IEnumerable<GetInvoicesByTenantResponse>>.IsSuccess(response, "success", "200"));
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                return Problem("An error occured on the server, please contact system administrator.");
            }
        }

    }
}
