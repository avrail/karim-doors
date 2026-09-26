using KarimDoors.Domain.Enums;

namespace KarimDoors.Application.Pricing;

public sealed record PricingComponentInput(
    string Code,
    string NameEn,
    string NameAr,
    string MaterialCode,
    string MaterialName,
    MaterialUnit Unit,
    decimal UnitPrice,
    int MaterialPriceVersion,
    MeasurementFormula Formula,
    decimal Quantity,
    decimal MeasurementMultiplier,
    decimal WastePercentage,
    DimensionSource LengthSource,
    decimal LengthOffsetMm,
    decimal? FixedLengthMm,
    DimensionSource WidthSource,
    decimal WidthOffsetMm,
    decimal? FixedWidthMm,
    decimal? ThicknessMm,
    decimal? FixedMeasurement,
    int Sequence);
