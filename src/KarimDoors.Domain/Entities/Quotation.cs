using KarimDoors.Domain.Common;
using KarimDoors.Domain.Enums;

namespace KarimDoors.Domain.Entities;

public sealed class Quotation : AuditableEntity
{
    public string Number { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
    public QuotationStatus Status { get; set; } = QuotationStatus.Draft;
    public int CurrentRevisionNumber { get; set; }

    public ICollection<QuotationRevision> Revisions { get; set; } = new List<QuotationRevision>();
}
