using KarimDoors.Domain.Common;
using KarimDoors.Domain.Enums;

namespace KarimDoors.Domain.Entities;

public sealed class Material : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public MaterialCategory Category { get; set; }
    public MaterialUnit Unit { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<MaterialPrice> Prices { get; set; } = new List<MaterialPrice>();
}
