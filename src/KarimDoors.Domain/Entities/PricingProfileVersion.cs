using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class PricingProfileVersion : EffectiveDatedEntity
{
    public int PricingProfileId { get; set; }
    public PricingProfile PricingProfile { get; set; } = null!;

    public decimal DefaultWastePercentage { get; set; }
    public decimal AdministrativePercentage { get; set; }
    public decimal ProfitPercentage { get; set; }
    public decimal ManufacturingCost { get; set; }
    public decimal TransportCost { get; set; }
    public decimal InstallationCost { get; set; }
    public string Currency { get; set; } = "EGP";
}
