using BillingSystem.Application.Requests;
using FluentValidation;

namespace BillingSystem.Application.Validators
{
    public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceRequest>
    {
        public UpdateInvoiceValidator()
        {
            RuleFor(x => x.InvoiceId).NotEmpty().WithMessage("{PropertyName} cannot be blank.");
            RuleFor(x => x.InvoiceStatus).NotEmpty().WithMessage("{PropertyName} cannot be blank.");
        }
    }
}
