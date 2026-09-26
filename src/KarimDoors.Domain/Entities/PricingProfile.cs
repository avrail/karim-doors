using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class PricingProfile : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<PricingProfileVersion> Versions { get; set; } = new List<PricingProfileVersion>();
}
