using KarimDoors.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Infrastructure.Persistence;

public sealed class KarimDoorsDbContext(DbContextOptions<KarimDoorsDbContext> options) : DbContext(options)
{
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<MaterialPrice> MaterialPrices => Set<MaterialPrice>();
    public DbSet<DoorTemplate> DoorTemplates => Set<DoorTemplate>();
    public DbSet<DoorTemplateVersion> DoorTemplateVersions => Set<DoorTemplateVersion>();
    public DbSet<DoorComponentRule> DoorComponentRules => Set<DoorComponentRule>();
    public DbSet<PricingProfile> PricingProfiles => Set<PricingProfile>();
    public DbSet<PricingProfileVersion> PricingProfileVersions => Set<PricingProfileVersion>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationRevision> QuotationRevisions => Set<QuotationRevision>();
    public DbSet<QuotationItem> QuotationItems => Set<QuotationItem>();
    public DbSet<CalculationSnapshot> CalculationSnapshots => Set<CalculationSnapshot>();
    public DbSet<SystemSettingDefinition> SystemSettingDefinitions => Set<SystemSettingDefinition>();
    public DbSet<SystemSettingVersion> SystemSettingVersions => Set<SystemSettingVersion>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Code).HasMaxLength(50);
            entity.Property(x => x.NameEn).HasMaxLength(200);
            entity.Property(x => x.NameAr).HasMaxLength(200);
        });

        modelBuilder.Entity<MaterialPrice>(entity =>
        {
            entity.HasIndex(x => new { x.MaterialId, x.Version }).IsUnique();
            entity.HasIndex(x => new { x.MaterialId, x.EffectiveFromUtc });
            entity.Property(x => x.UnitPrice).HasPrecision(18, 4);
            entity.Property(x => x.Currency).HasMaxLength(3);
            entity.Property(x => x.ChangeReason).HasMaxLength(500);
        });

        modelBuilder.Entity<DoorTemplate>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.ProjectId);
            entity.HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
            entity.Property(x => x.Code).HasMaxLength(50);
            entity.Property(x => x.NameEn).HasMaxLength(200);
            entity.Property(x => x.NameAr).HasMaxLength(200);
        });

        modelBuilder.Entity<DoorTemplateVersion>(entity =>
        {
            entity.HasIndex(x => new { x.DoorTemplateId, x.Version }).IsUnique();
            entity.HasIndex(x => new { x.DoorTemplateId, x.EffectiveFromUtc });
            entity.Property(x => x.ChangeReason).HasMaxLength(500);
            entity.Property(x => x.SourceReference).HasMaxLength(500);
        });

        modelBuilder.Entity<DoorComponentRule>(entity =>
        {
            entity.HasIndex(x => new { x.DoorTemplateVersionId, x.Code }).IsUnique();
            entity.Property(x => x.Code).HasMaxLength(50);
            entity.Property(x => x.NameEn).HasMaxLength(200);
            entity.Property(x => x.NameAr).HasMaxLength(200);
            entity.Property(x => x.Quantity).HasPrecision(18, 6);
            entity.Property(x => x.MeasurementMultiplier).HasPrecision(18, 6);
            entity.Property(x => x.WastePercentage).HasPrecision(9, 4);
            entity.Property(x => x.LengthOffsetMm).HasPrecision(18, 4);
            entity.Property(x => x.WidthOffsetMm).HasPrecision(18, 4);
            entity.Property(x => x.FixedLengthMm).HasPrecision(18, 4);
            entity.Property(x => x.FixedWidthMm).HasPrecision(18, 4);
            entity.Property(x => x.ThicknessMm).HasPrecision(18, 4);
            entity.Property(x => x.FixedMeasurement).HasPrecision(18, 6);
        });

        modelBuilder.Entity<PricingProfile>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Code).HasMaxLength(50);
            entity.Property(x => x.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<PricingProfileVersion>(entity =>
        {
            entity.HasIndex(x => new { x.PricingProfileId, x.Version }).IsUnique();
            entity.Property(x => x.DefaultWastePercentage).HasPrecision(9, 4);
            entity.Property(x => x.AdministrativePercentage).HasPrecision(9, 4);
            entity.Property(x => x.ProfitPercentage).HasPrecision(9, 4);
            entity.Property(x => x.ManufacturingCost).HasPrecision(18, 4);
            entity.Property(x => x.TransportCost).HasPrecision(18, 4);
            entity.Property(x => x.InstallationCost).HasPrecision(18, 4);
            entity.Property(x => x.Currency).HasMaxLength(3);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Code).HasMaxLength(50);
            entity.Property(x => x.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasIndex(x => new { x.CustomerId, x.Code }).IsUnique();
            entity.Property(x => x.Code).HasMaxLength(50);
            entity.Property(x => x.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Quotation>(entity =>
        {
            entity.HasIndex(x => x.Number).IsUnique();
            entity.Property(x => x.Number).HasMaxLength(50);
        });

        modelBuilder.Entity<QuotationRevision>(entity =>
        {
            entity.HasIndex(x => new { x.QuotationId, x.RevisionNumber }).IsUnique();
        });

        modelBuilder.Entity<QuotationItem>(entity =>
        {
            entity.Property(x => x.Quantity).HasPrecision(18, 4);
            entity.Property(x => x.CalculatedUnitPrice).HasPrecision(18, 4);
            entity.Property(x => x.OverrideUnitPrice).HasPrecision(18, 4);
            entity.HasOne(x => x.Snapshot)
                .WithOne(x => x.QuotationItem)
                .HasForeignKey<CalculationSnapshot>(x => x.QuotationItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CalculationSnapshot>(entity =>
        {
            entity.Property(x => x.MaterialCost).HasPrecision(18, 4);
            entity.Property(x => x.DryCost).HasPrecision(18, 4);
            entity.Property(x => x.AdministrativeCost).HasPrecision(18, 4);
            entity.Property(x => x.ProfitCost).HasPrecision(18, 4);
            entity.Property(x => x.CalculatedPrice).HasPrecision(18, 4);
        });

        modelBuilder.Entity<SystemSettingDefinition>(entity =>
        {
            entity.HasIndex(x => x.Key).IsUnique();
            entity.Property(x => x.Key).HasMaxLength(100);
            entity.Property(x => x.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<SystemSettingVersion>(entity =>
        {
            entity.HasIndex(x => new { x.SystemSettingDefinitionId, x.Version }).IsUnique();
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(x => x.OccurredOnUtc);
            entity.Property(x => x.EntityName).HasMaxLength(200);
            entity.Property(x => x.EntityKey).HasMaxLength(200);
            entity.Property(x => x.Action).HasMaxLength(50);
        });
    }
}
