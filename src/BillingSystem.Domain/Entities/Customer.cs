using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public partial class Customer : IAggregateRoot
    {
        public long Id { get; set; }
        public string FullName { get; set; } = null!;
        public string CustomerType { get; set; } = null!;
        public string AddressCountry { get; set; } = null!;
        public string AddressCity { get; set; } = null!;
        public string AddressPostalCode { get; set; } = null!;
        public string AddressDistrict { get; set; } = null!;
        public string? AddressStreet { get; set; }
        public string? AddressBuildingNumber { get; set; }
        public string? AddressSubNumber { get; set; }
        public string IdentityNumber { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? Vatnumber { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public int? IdentityType { get; set; }
        public decimal? OverpaymentBalance { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
