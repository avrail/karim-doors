using KarimDoors.Domain.Enums;

namespace KarimDoors.Application.Pricing;

public sealed record PricingComponentResult(
    string Code,
    string NameEn,
    string NameAr,
    string MaterialCode,
    MaterialUnit Unit,
    decimal Measurement,
    decimal UnitPrice,
    decimal BaseCost,
    decimal WastePercentage,
    decimal WasteCost,
    decimal TotalCost,
    int MaterialPriceVersion,
    int Sequence);
