using BillingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Infrastructure.EF;

public partial class BillingServiceDBContext : DbContext
{
    public BillingServiceDBContext()
    {
    }

    public BillingServiceDBContext(DbContextOptions<BillingServiceDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditEvent> AuditEvents { get; set; }

    public virtual DbSet<InvoiceSequenceValue> InvoiceSequenceValues { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoicesAttachment> InvoicesAttachments { get; set; }

    public virtual DbSet<InvoicesItem> InvoicesItems { get; set; }

    public virtual DbSet<InvoicesLog> InvoicesLogs { get; set; }

    public virtual DbSet<OutboxEvent> OutboxEvents { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentNotificationTemplate> PaymentNotificationTemplates { get; set; }

    public virtual DbSet<ProductsPrice> ProductsPrices { get; set; }

    public virtual DbSet<TaxSetting> TaxSettings { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:ReadWriteConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AuditEvent");

            entity.ToTable("AuditEvents", "audit");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EntityId).HasMaxLength(20);
            entity.Property(e => e.EntityType).HasMaxLength(100);
            entity.Property(e => e.UserId).HasMaxLength(100);
        });

        modelBuilder.Entity<InvoiceSequenceValue>().HasNoKey();

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.AddressBuildingNumber)
                .HasMaxLength(200)
                .HasColumnName("Address_BuildingNumber");
            entity.Property(e => e.AddressCity)
                .HasMaxLength(200)
                .HasColumnName("Address_City");
            entity.Property(e => e.AddressCountry)
                .HasMaxLength(200)
                .HasColumnName("Address_Country");
            entity.Property(e => e.AddressDistrict)
                .HasMaxLength(200)
                .HasColumnName("Address_District");
            entity.Property(e => e.AddressPostalCode)
                .HasMaxLength(50)
                .HasColumnName("Address_PostalCode");
            entity.Property(e => e.AddressStreet)
                .HasMaxLength(200)
                .HasColumnName("Address_Street");
            entity.Property(e => e.AddressSubNumber)
                .HasMaxLength(200)
                .HasColumnName("Address_SubNumber");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerType).HasMaxLength(100);
            entity.Property(e => e.EmailAddress).HasMaxLength(80);
            entity.Property(e => e.FullName).HasMaxLength(500);
            entity.Property(e => e.IdentityNumber).HasMaxLength(100);
            entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.OverpaymentBalance).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Vatnumber)
                .HasMaxLength(100)
                .HasColumnName("VATNumber");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(e => e.ExternalId, "IX_Invoices_ExternalId");

            entity.HasIndex(e => e.ExternalId, "UX_Invoices_ExternalId").IsUnique();

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CancelationReasons).HasMaxLength(3000);
            entity.Property(e => e.ClientReferenceNumber).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Currency).HasMaxLength(50);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.DiscountedPrimaryInvoiceId).HasColumnName("DiscountedPrimaryInvoiceID");
            entity.Property(e => e.DueAt).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
            entity.Property(e => e.IssuedAt).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentProvider).HasMaxLength(100);
            entity.Property(e => e.PaymentReference).HasMaxLength(100);
            entity.Property(e => e.PrimaryInvoiceId).HasColumnName("PrimaryInvoiceID");
            entity.Property(e => e.SubstitutionPrimaryInvoiceId).HasColumnName("SubstitutionPrimaryInvoiceID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Customers");

            entity.HasOne(d => d.DiscountedPrimaryInvoice).WithMany(p => p.InverseDiscountedPrimaryInvoice)
                .HasForeignKey(d => d.DiscountedPrimaryInvoiceId)
                .HasConstraintName("FK_Invoices_Invoices2");

            entity.HasOne(d => d.PrimaryInvoice).WithMany(p => p.InversePrimaryInvoice)
                .HasForeignKey(d => d.PrimaryInvoiceId)
                .HasConstraintName("FK_Invoices_Invoices");

            entity.HasOne(d => d.SubstitutionPrimaryInvoice).WithMany(p => p.InverseSubstitutionPrimaryInvoice)
                .HasForeignKey(d => d.SubstitutionPrimaryInvoiceId)
                .HasConstraintName("FK_Invoices_Invoices1");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Tenants");
        });

        modelBuilder.Entity<InvoicesAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_InvoiceAttachments");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(200);

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoicesAttachments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoicesAttachments_Invoices");
        });

        modelBuilder.Entity<InvoicesItem>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ItemDesc).HasMaxLength(500);
            entity.Property(e => e.ItemName).HasMaxLength(200);

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoicesItems)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoicesItems_Invoices");
        });

        modelBuilder.Entity<InvoicesLog>(entity =>
        {
            entity.ToTable("Invoices_Log");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Currency).HasMaxLength(50);
            entity.Property(e => e.DueAt).HasColumnType("datetime");
            entity.Property(e => e.IssuedAt).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentProvider).HasMaxLength(100);
            entity.Property(e => e.PaymentReference).HasMaxLength(100);

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoicesLogs)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Log_Invoices");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvoicesLogs)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Log_Tenants");
        });

        modelBuilder.Entity<OutboxEvent>(entity =>
        {
            entity.ToTable("Outbox_events");

            entity.Property(e => e.AggregateId).HasColumnName("Aggregate_id");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EventType)
                .HasMaxLength(300)
                .HasColumnName("Event_type");
            entity.Property(e => e.ProcessingStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LastError)
                .HasMaxLength(1000)
                .HasColumnName("Last_error");
            entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.ProviderReference)
                .HasMaxLength(50)
                .HasColumnName("Provider_reference");
            entity.Property(e => e.ProviderResponse).HasMaxLength(1000);

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Invoices");
        });

        modelBuilder.Entity<PaymentNotificationTemplate>(entity =>
        {
            entity.ToTable("PaymentNotificationTemplates", "config");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Template).HasColumnType("ntext");
            entity.Property(e => e.TemplateName).HasMaxLength(100);
        });

        modelBuilder.Entity<ProductsPrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ProductsTypes");

            entity.ToTable("ProductsPrices", "config");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MaximumAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MinimumAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ReferenceNumber).HasMaxLength(100);
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<TaxSetting>(entity =>
        {
            entity.ToTable("TaxSettings", "config");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TaxName).HasMaxLength(100);
            entity.Property(e => e.TaxPercentage).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.Property(e => e.TenantGuid).HasColumnName("TenantGUID");
            entity.Property(e => e.TenantName).HasMaxLength(100);
            entity.Property(e => e.ValidFrom).HasColumnType("datetime");
            entity.Property(e => e.ValidTo).HasColumnType("datetime");
        });
        modelBuilder.HasSequence("InvoiceNumberSequence");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
