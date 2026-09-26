using KarimDoors.Domain.Common;

namespace KarimDoors.Domain.Entities;

public sealed class CalculationSnapshot : Entity
{
    public int QuotationItemId { get; set; }
    public QuotationItem QuotationItem { get; set; } = null!;
    public DateTime CapturedOnUtc { get; set; } = DateTime.UtcNow;
    public string InputJson { get; set; } = "{}";
    public string BreakdownJson { get; set; } = "[]";
    public string ConfigurationJson { get; set; } = "{}";
    public decimal MaterialCost { get; set; }
    public decimal DryCost { get; set; }
    public decimal AdministrativeCost { get; set; }
    public decimal ProfitCost { get; set; }
    public decimal CalculatedPrice { get; set; }
}
