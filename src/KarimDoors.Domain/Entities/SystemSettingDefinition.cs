using KarimDoors.Domain.Common;
using KarimDoors.Domain.Enums;

namespace KarimDoors.Domain.Entities;

public sealed class SystemSettingDefinition : AuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SettingValueType ValueType { get; set; }
    public bool IsPricingSensitive { get; set; }
    public ICollection<SystemSettingVersion> Versions { get; set; } = new List<SystemSettingVersion>();
}
