using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class QuotationItem : AuditableEntity
{
    public int QuotationRevisionId { get; set; }
    public QuotationRevision QuotationRevision { get; set; } = null!;
    public int DoorTemplateId { get; set; }
    public DoorTemplate DoorTemplate { get; set; } = null!;
    public int WidthMm { get; set; }
    public int HeightMm { get; set; }
    public decimal Quantity { get; set; } = 1m;
    public decimal CalculatedUnitPrice { get; set; }
    public decimal? OverrideUnitPrice { get; set; }
    public string? OverrideReason { get; set; }

    public CalculationSnapshot Snapshot { get; set; } = null!;

    public decimal FinalUnitPrice => OverrideUnitPrice ?? CalculatedUnitPrice;
}
