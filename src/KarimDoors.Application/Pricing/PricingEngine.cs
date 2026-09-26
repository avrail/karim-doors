using KarimDoors.Domain.Enums;

namespace KarimDoors.Application.Pricing;

public sealed class PricingEngine
{
    public PricingResult Calculate(PricingContext context, decimal requestedQuantity)
    {
        if (context.WidthMm <= 0)
            throw new ArgumentOutOfRangeException(nameof(context.WidthMm));
        if (context.HeightMm <= 0)
            throw new ArgumentOutOfRangeException(nameof(context.HeightMm));
        if (requestedQuantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(requestedQuantity));

        var componentResults = context.Components
            .OrderBy(x => x.Sequence)
            .Select(x => CalculateComponent(context, x))
            .ToList();

        var materialCost = componentResults.Sum(x => x.TotalCost);
        var manufacturing = context.Profile.ManufacturingCost;
        var transport = context.Profile.TransportCost;
        var installation = context.Profile.InstallationCost;
        var dryCost = materialCost + manufacturing + transport + installation;
        var administrativeCost = dryCost * context.Profile.AdministrativePercentage / 100m;
        var profitCost = dryCost * context.Profile.ProfitPercentage / 100m;
        var unitCalculatedPrice = dryCost + administrativeCost + profitCost;

        return new PricingResult(
            context.DoorTemplateCode,
            context.DoorTemplateName,
            context.DoorTemplateVersion,
            context.WidthMm,
            context.HeightMm,
            context.FireRatingMinutes,
            context.Profile.Code,
            context.Profile.Version,
            context.Profile.Currency,
            context.CalculationDateUtc,
            requestedQuantity,
            RoundMoney(materialCost),
            RoundMoney(manufacturing),
            RoundMoney(transport),
            RoundMoney(installation),
            RoundMoney(dryCost),
            context.Profile.AdministrativePercentage,
            RoundMoney(administrativeCost),
            context.Profile.ProfitPercentage,
            RoundMoney(profitCost),
            RoundMoney(unitCalculatedPrice),
            RoundMoney(unitCalculatedPrice * requestedQuantity),
            componentResults);
    }

    private static PricingComponentResult CalculateComponent(
        PricingContext context,
        PricingComponentInput component)
    {
        var measurement = CalculateMeasurement(context, component);
        var baseCost = measurement * component.UnitPrice;
        var wasteCost = baseCost * component.WastePercentage / 100m;
        var totalCost = baseCost + wasteCost;

        return new PricingComponentResult(
            component.Code,
            component.NameEn,
            component.NameAr,
            component.MaterialCode,
            component.Unit,
            RoundMeasurement(measurement),
            component.UnitPrice,
            baseCost,
            component.WastePercentage,
            wasteCost,
            totalCost,
            component.MaterialPriceVersion,
            component.Sequence);
    }

    private static decimal CalculateMeasurement(
        PricingContext context,
        PricingComponentInput component)
    {
        var heightM = context.HeightMm / 1000m;
        var widthM = context.WidthMm / 1000m;
        var multiplier = component.MeasurementMultiplier == 0m
            ? 1m
            : component.MeasurementMultiplier;

        return component.Formula switch
        {
            MeasurementFormula.RectangularVolume =>
                ResolveDimensionMm(component.LengthSource, component.FixedLengthMm, component.LengthOffsetMm, context) / 1000m
                * ResolveDimensionMm(component.WidthSource, component.FixedWidthMm, component.WidthOffsetMm, context) / 1000m
                * RequirePositive(component.ThicknessMm, component.Code, nameof(component.ThicknessMm)) / 1000m
                * component.Quantity
                * multiplier,

            MeasurementFormula.RectangularArea =>
                ResolveDimensionMm(component.LengthSource, component.FixedLengthMm, component.LengthOffsetMm, context) / 1000m
                * ResolveDimensionMm(component.WidthSource, component.FixedWidthMm, component.WidthOffsetMm, context) / 1000m
                * component.Quantity
                * multiplier,

            MeasurementFormula.LinearLength =>
                ResolveDimensionMm(component.LengthSource, component.FixedLengthMm, component.LengthOffsetMm, context) / 1000m
                * component.Quantity
                * multiplier,

            MeasurementFormula.Quantity => component.Quantity * multiplier,

            MeasurementFormula.DoorSurfaceWithEdges =>
                ((2m * heightM * widthM)
                 + ((2m * heightM + widthM)
                    * RequirePositive(component.FixedWidthMm, component.Code, nameof(component.FixedWidthMm)) / 1000m))
                * multiplier,

            MeasurementFormula.ArchitravePerimeter =>
                (2m * ((context.HeightMm + component.LengthOffsetMm) / 1000m)
                 + 2m * ((context.WidthMm + component.WidthOffsetMm) / 1000m))
                * component.Quantity
                * multiplier,

            MeasurementFormula.FixedMeasurement =>
                RequirePositive(component.FixedMeasurement, component.Code, nameof(component.FixedMeasurement))
                * component.Quantity
                * multiplier,

            _ => throw new NotSupportedException($"Unsupported formula {component.Formula}.")
        };
    }

    private static decimal ResolveDimensionMm(
        DimensionSource source,
        decimal? fixedValueMm,
        decimal offsetMm,
        PricingContext context) => source switch
    {
        DimensionSource.Fixed => RequirePositive(fixedValueMm, "dimension", nameof(fixedValueMm)),
        DimensionSource.DoorWidth => context.WidthMm + offsetMm,
        DimensionSource.DoorHeight => context.HeightMm + offsetMm,
        _ => throw new NotSupportedException($"Unsupported dimension source {source}.")
    };

    private static decimal RequirePositive(decimal? value, string componentCode, string fieldName)
    {
        if (value is null || value <= 0m)
            throw new InvalidOperationException($"{componentCode}: {fieldName} must be greater than zero.");
        return value.Value;
    }

    private static decimal RoundMoney(decimal value) =>
        decimal.Round(value, 2, MidpointRounding.AwayFromZero);

    private static decimal RoundMeasurement(decimal value) =>
        decimal.Round(value, 6, MidpointRounding.AwayFromZero);
}
