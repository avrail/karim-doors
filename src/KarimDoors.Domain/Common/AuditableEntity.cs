namespace KarimDoors.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    public string? ModifiedBy { get; set; }
}
