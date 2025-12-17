using AutoMapper;
using BillingSystem.Application.Requests;
using BillingSystem.Application.Responses;
using BillingSystem.Domain.Entities;

namespace BillingSystem.Application.MapperRules
{
    public class InvoiceMapperProfile : Profile
    {
        public InvoiceMapperProfile()
        {

            CreateMap<InvoiceItems, InvoicesItem>()
               .ForMember(s => s.CreatedDate, o => o.MapFrom(s => s.CreatedDate))
               .ForMember(s => s.Quantity, o => o.MapFrom(s => s.Quantity))
               .ForMember(s => s.ItemDesc, o => o.MapFrom(s => s.ItemDesc))
               .ForMember(s => s.ItemName, o => o.MapFrom(s => s.ItemName))
               .ForMember(s => s.Amount, o => o.MapFrom(s => s.Amount))
               .ForMember(s => s.IsDeleted, o => o.MapFrom(s => false))
               .ForMember(s => s.CreatedBy, o => o.MapFrom(s => "Tenant"))
               .ReverseMap();

            CreateMap<InvoiceAttachments, InvoicesAttachment>()
               .ForMember(s => s.CreatedDate, o => o.MapFrom(s => s.CreatedDate))
               .ForMember(s => s.FileName, o => o.MapFrom(s => s.FileName))
               .ForMember(s => s.CreatedDate, o => o.MapFrom(s => DateTime.Now))
               .ForMember(s => s.IsDeleted, o => o.MapFrom(s => false))
               .ForMember(s => s.CreatedBy, o => o.MapFrom(s => "Tenant"))
               .ReverseMap();

            CreateMap<InvoiceCustomer, Customer>()
                .ForMember(s => s.AddressCountry, o => o.MapFrom(s => s.Address.AddressCountry))
                .ForMember(s => s.AddressBuildingNumber, o => o.MapFrom(s => s.Address.AddressBuildingNumber))
                .ForMember(s => s.AddressSubNumber, o => o.MapFrom(s => s.Address.AddressSubNumber))
                .ForMember(s => s.AddressStreet, o => o.MapFrom(s => s.Address.AddressStreet))
                .ForMember(s => s.AddressPostalCode, o => o.MapFrom(s => s.Address.AddressPostalCode))
                .ForMember(s => s.AddressCity, o => o.MapFrom(s => s.Address.AddressCity))
                .ForMember(s => s.AddressDistrict, o => o.MapFrom(s => s.Address.AddressDistrict))
                .ForMember(s => s.FullName, o => o.MapFrom(s => s.FullName))
                .ForMember(s => s.IdentityNumber, o => o.MapFrom(s => s.IdentityNumber))
                .ForMember(s => s.PhoneNumber, o => o.MapFrom(s => s.PhoneNumber))
                .ForMember(s => s.EmailAddress, o => o.MapFrom(s => s.EmailAddress))
                .ForMember(s => s.CustomerType, o => o.MapFrom(s => s.CustomerType.ToString()))
                .ForMember(s => s.IdentityType, o => o.MapFrom(s => s.IdentityType))
                .ForMember(s => s.CreatedBy, o => o.MapFrom(s => "Tenant"))
                .ReverseMap();

            CreateMap<CreateInvoiceRequest, Invoice>()
                .ForMember(s => s.IssuedAt , o => o.MapFrom(s => s.IssuedAt))
                .ForMember(s => s.InvoiceStatus, o => o.MapFrom(s => s.InvoiceStatus))
                .ForMember(s => s.Metadata, o => o.MapFrom(s => s.Metadata))
                .ForMember(s => s.Currency, o => o.MapFrom(s => s.Currency))
                .ForMember(s => s.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(s => s.ClientReferenceNumber, o => o.MapFrom(s => s.ClientReferenceNumber))
                .ForMember(s => s.DueAt, o => o.MapFrom(s => s.DueAt))
                .ForMember(s => s.ExternalId, o => o.MapFrom(s => s.ExternalId))
                .ForMember(s => s.InvoiceType, o => o.MapFrom(s => s.InvoiceType))
                .ForMember(s => s.TenantId, o => o.MapFrom(s => s.TenantId))
                .ForMember(s => s.IsSubInvoice, o => o.MapFrom(s => s.IsSubInvoice))
                //.ForMember(s => s.PrimaryInvoiceId, o => o.MapFrom(s => s.PrimaryInvoiceId))
                .ForMember(s => s.CreatedBy, o => o.MapFrom(s => s.TenantId))
                .ReverseMap();

            CreateMap<Customer, GetInvoicesByTenantCustomer>().ReverseMap();
            CreateMap<Invoice, GetInvoicesByTenantResponse>()
                .ForMember(s => s.IssuedAt, o => o.MapFrom(s => s.IssuedAt))
                .ForMember(s => s.ClientReferenceNumber, o => o.MapFrom(s => s.ClientReferenceNumber))
                .ForMember(s => s.CreatedDate, o => o.MapFrom(s => s.CreatedDate))
                .ForMember(s => s.LastUpdatedDate, o => o.MapFrom(s => s.LastUpdatedDate))
                .ForMember(s => s.TenantId, o => o.MapFrom(s => s.TenantId))
                .ForMember(s => s.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(s => s.CancelationReasons, o => o.MapFrom(s => s.CancelationReasons))
                .ForMember(s => s.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                .ForMember(s => s.Currency, o => o.MapFrom(s => s.Currency))
                //.ForMember(s => s.Customer.IdentityNumber, o => o.MapFrom(s => s.Customer.IdentityNumber))
                //.ForMember(s => s.Customer.PhoneNumber, o => o.MapFrom(s => s.Customer.PhoneNumber))
                //.ForMember(s => s.Customer.AddressSubNumber, o => o.MapFrom(s => s.Customer.AddressSubNumber))
                //.ForMember(s => s.Customer.FullName, o => o.MapFrom(s => s.Customer.FullName))
                //.ForMember(s => s.Customer.AddressBuildingNumber, o => o.MapFrom(s => s.Customer.AddressBuildingNumber))
                //.ForMember(s => s.Customer.AddressCity, o => o.MapFrom(s => s.Customer.AddressCity))
                //.ForMember(s => s.Customer.AddressCountry, o => o.MapFrom(s => s.Customer.AddressCountry))
                //.ForMember(s => s.Customer.AddressDistrict, o => o.MapFrom(s => s.Customer.AddressDistrict))
                //.ForMember(s => s.Customer.AddressPostalCode, o => o.MapFrom(s => s.Customer.AddressPostalCode))
                //.ForMember(s => s.Customer.AddressStreet, o => o.MapFrom(s => s.Customer.AddressStreet))
                //.ForMember(s => s.Customer.OverpaymentBalance, o => o.MapFrom(s => s.Customer.OverpaymentBalance))
                //.ForMember(s => s.Customer.Vatnumber, o => o.MapFrom(s => s.Customer.Vatnumber))
                .ReverseMap();


          
        }
    }
}
