using KarimDoors.Application.Pricing;
using KarimDoors.Domain.Enums;
using Xunit;

namespace KarimDoors.Application.Tests;

public sealed class PricingEngineTests
{
    [Fact]
    public void HassanAllamD04_Matches_SourceWorkbook()
    {
        var context = BuildD04Context();
        var result = new PricingEngine().Calculate(context, 1m);

        Assert.Equal(3496.84m, result.MaterialCost);
        Assert.Equal(4561.84m, result.DryCost);
        Assert.Equal(798.32m, result.AdministrativeCost);
        Assert.Equal(1140.46m, result.ProfitCost);
        Assert.Equal(6500.63m, result.UnitCalculatedPrice);
    }

    [Fact]
    public void Quantity_Only_Multiplies_Final_Total_Not_Unit_Price()
    {
        var context = BuildD04Context();
        var result = new PricingEngine().Calculate(context, 10m);

        Assert.Equal(6500.63m, result.UnitCalculatedPrice);
        Assert.Equal(65006.29m, result.TotalCalculatedPrice);
    }

    private static PricingContext BuildD04Context()
    {
        var components = new List<PricingComponentInput>
        {
            Timber("FRAME-V", 1, 9500, 2, DimensionSource.DoorHeight, 0, 150, 50),
            Timber("FRAME-H", 2, 9500, 1, DimensionSource.DoorWidth, 0, 150, 50),
            Timber("ARCH-V", 3, 9500, 4, DimensionSource.DoorHeight, 80, 75, 25),
            Timber("ARCH-H", 4, 9500, 2, DimensionSource.DoorWidth, 160, 75, 25),
            Timber("OAK-V", 5, 23500, 2, DimensionSource.DoorHeight, -40, 50, 12.5m, "TIMBER-OAK"),
            Timber("OAK-H", 6, 23500, 2, DimensionSource.DoorWidth, -70, 50, 12.5m, "TIMBER-OAK"),
            Timber("LEAF-V", 7, 9500, 2, DimensionSource.DoorHeight, -40, 125, 38),
            Timber("LEAF-H", 8, 9500, 2, DimensionSource.DoorWidth, -70, 125, 38),
            Timber("CROSS", 9, 9500, 21, DimensionSource.DoorWidth, -250, 50, 38),
            Timber("COMP-V", 10, 9500, 2, DimensionSource.DoorHeight, 0, 50, 25),
            Timber("COMP-H", 11, 9500, 1, DimensionSource.DoorWidth, 0, 50, 25),
            Quantity("MDF6", 12, "MDF-6", MaterialUnit.Piece, 215, 2),
            Quantity("MDF18", 13, "MDF-18", MaterialUnit.Piece, 450, .5m),
            Surface("VENEER", 14, "VENEER-OAK", MaterialUnit.SquareMeter, 55, 1.35m),
            Quantity("FOAM", 15, "FOAM", MaterialUnit.Piece, 75, 1),
            new PricingComponentInput("GASKET", "Weather gasket", "Weather gasket", "WEATHER-GASKET", "Weather gasket", MaterialUnit.LinearMeter, 15, 1, MeasurementFormula.ArchitravePerimeter, 1, 1, 0, DimensionSource.Fixed, -40, null, DimensionSource.Fixed, -70, null, null, null, 16),
            Surface("PAINT", 17, "PAINT", MaterialUnit.SquareMeter, 75, 1.2m),
            Quantity("PACKAGING", 18, "PACKAGING", MaterialUnit.Kilogram, 22.5m, 2.3m)
        };

        return new PricingContext(
            "HA-D04",
            "Hassan Allam D04",
            1,
            0,
            970,
            2200,
            new PricingProfileInput("HA-2022", 1, 17.5m, 25m, 475m, 115m, 475m, "EGP"),
            components,
            new DateTime(2022, 1, 22, 0, 0, 0, DateTimeKind.Utc));
    }

    private static PricingComponentInput Timber(
        string code, int sequence, decimal unitPrice, decimal quantity,
        DimensionSource lengthSource, decimal lengthOffsetMm,
        decimal widthMm, decimal thicknessMm, string materialCode = "TIMBER-MOUSKI") =>
        new(code, code, code, materialCode, materialCode, MaterialUnit.CubicMeter, unitPrice, 1,
            MeasurementFormula.RectangularVolume, quantity, 1m, 15m,
            lengthSource, lengthOffsetMm, null,
            DimensionSource.Fixed, 0m, widthMm,
            thicknessMm, null, sequence);

    private static PricingComponentInput Quantity(
        string code, int sequence, string materialCode, MaterialUnit unit,
        decimal unitPrice, decimal quantity) =>
        new(code, code, code, materialCode, materialCode, unit, unitPrice, 1,
            MeasurementFormula.Quantity, quantity, 1m, 0m,
            DimensionSource.Fixed, 0m, null,
            DimensionSource.Fixed, 0m, null,
            null, null, sequence);

    private static PricingComponentInput Surface(
        string code, int sequence, string materialCode, MaterialUnit unit,
        decimal unitPrice, decimal multiplier) =>
        new(code, code, code, materialCode, materialCode, unit, unitPrice, 1,
            MeasurementFormula.DoorSurfaceWithEdges, 1m, multiplier, 0m,
            DimensionSource.Fixed, 0m, null,
            DimensionSource.Fixed, 0m, 500m,
            null, null, sequence);
}
