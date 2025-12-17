using BillingSystem.Application.Requests;
using FluentValidation;

namespace BillingSystem.Application.Validators
{
    public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequest>
    {
        public CreateInvoiceValidator()
        {
            RuleFor(x => x.Amount).NotEmpty().WithMessage("{PropertyName} cannot be blank.")
                                  .GreaterThan(0).WithMessage("{PropertyName} cannot be less than or equal to 0.");

            RuleFor(x => x.ExternalId).NotEmpty().WithMessage("{PropertyName} cannot be blank.");
            RuleFor(x => x.Currency).NotEmpty().WithMessage("{PropertyName} cannot be blank.");
            RuleFor(x => x.InvoiceType).NotEmpty().WithMessage("{PropertyName} cannot be blank.");
            RuleFor(x => x.Customer).SetValidator(new CustomerValidator());


        }
    }
    public class CustomerValidator : AbstractValidator<InvoiceCustomer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.CustomerType).NotEmpty().WithMessage("{PropertyName} cannot be blank.")
                                        .IsInEnum().WithMessage("{PropertyName} must be 1 or 2");

            RuleFor(x => x.IdentityNumber).NotEmpty().WithMessage("{PropertyName} cannot be blank.");
            RuleFor(x => x.EmailAddress).EmailAddress().WithMessage("{PropertyName} not valid email address");
            RuleFor(x => x.Address).SetValidator(new CustomerAddressValidator());

        }
    }
    public class CustomerAddressValidator : AbstractValidator<CustomerAddress>
    {
        public CustomerAddressValidator()
        {
            RuleFor(x => x.AddressCountry).NotEmpty().WithMessage("{PropertyName} cannot be blank.");
            RuleFor(x => x.AddressCity).NotEmpty().WithMessage("{PropertyName} cannot be blank.");
            RuleFor(x => x.AddressDistrict).NotEmpty().WithMessage("{PropertyName} cannot be blank.");
            RuleFor(x => x.AddressPostalCode).NotEmpty().WithMessage("{PropertyName} cannot be blank.");
        }
    }

}
