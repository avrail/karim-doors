using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class DoorTemplateVersion : EffectiveDatedEntity
{
    public int DoorTemplateId { get; set; }
    public DoorTemplate DoorTemplate { get; set; } = null!;
    public int DefaultWidthMm { get; set; }
    public int DefaultHeightMm { get; set; }
    public int FireRatingMinutes { get; set; }
    public string? SourceReference { get; set; }

    public ICollection<DoorComponentRule> Components { get; set; } = new List<DoorComponentRule>();
}
