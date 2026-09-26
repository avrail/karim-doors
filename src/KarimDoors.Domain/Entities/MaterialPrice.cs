using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class MaterialPrice : EffectiveDatedEntity
{
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "EGP";
    public string? SupplierReference { get; set; }
}
