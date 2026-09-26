using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class DoorTemplate : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
    public string? PricingProfileCode { get; set; }
    public int QuoteRoundingDigits { get; set; } = 2;

    public ICollection<DoorTemplateVersion> Versions { get; set; } = new List<DoorTemplateVersion>();
}
