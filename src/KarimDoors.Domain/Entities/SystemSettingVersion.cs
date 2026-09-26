using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class SystemSettingVersion : EffectiveDatedEntity
{
    public int SystemSettingDefinitionId { get; set; }
    public SystemSettingDefinition SystemSettingDefinition { get; set; } = null!;
    public string Value { get; set; } = string.Empty;
}
