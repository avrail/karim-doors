using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class QuotationRevision : AuditableEntity
{
    public int QuotationId { get; set; }
    public Quotation Quotation { get; set; } = null!;
    public int RevisionNumber { get; set; }
    public DateTime CalculationDateUtc { get; set; }
    public DateTime? ApprovedOnUtc { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Notes { get; set; }

    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
}
