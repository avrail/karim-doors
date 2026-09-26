namespace KarimDoors.Domain.Common;

public abstract class EffectiveDatedEntity : AuditableEntity
{
    public int Version { get; set; } = 1;
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    public string? ChangeReason { get; set; }

    public bool IsEffectiveAt(DateTime instantUtc) =>
        EffectiveFromUtc <= instantUtc &&
        (EffectiveToUtc is null || EffectiveToUtc > instantUtc);
}
