using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class Project : AuditableEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
