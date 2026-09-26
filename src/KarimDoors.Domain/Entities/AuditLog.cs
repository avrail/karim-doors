using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class AuditLog : Entity
{
    public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityKey { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? Reason { get; set; }
}
