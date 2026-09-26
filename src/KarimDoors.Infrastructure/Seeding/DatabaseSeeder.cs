using KarimDoors.Domain.Entities;
using KarimDoors.Domain.Enums;
using KarimDoors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Infrastructure.Seeding;

public sealed class DatabaseSeeder(KarimDoorsDbContext dbContext)
{
    private static readonly DateTime SourceEffectiveDateUtc = new(2022, 1, 22, 0, 0, 0, DateTimeKind.Utc);

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existingTemplate = await dbContext.DoorTemplates
            .SingleOrDefaultAsync(x => x.Code == "HA-D04", cancellationToken);
        if (existingTemplate is not null &&
            await dbContext.DoorTemplateVersions.AnyAsync(x => x.DoorTemplateId == existingTemplate.Id, cancellationToken))
            return;
        if (existingTemplate is null && await dbContext.DoorTemplates.AnyAsync(cancellationToken))
            return;

        var existingMaterials = existingTemplate is null
            ? null
            : await dbContext.Materials.ToDictionaryAsync(x => x.Code, cancellationToken);

        var mouski = existingMaterials is null ? Material("TIMBER-MOUSKI", "Mouski timber", "خشب موسكي", MaterialCategory.Timber, MaterialUnit.CubicMeter, 9500m) : existingMaterials["TIMBER-MOUSKI"];
        var oak = existingMaterials is null ? Material("TIMBER-OAK", "Oak timber", "خشب أرو", MaterialCategory.Timber, MaterialUnit.CubicMeter, 23500m) : existingMaterials["TIMBER-OAK"];
        var mdf6 = existingMaterials is null ? Material("MDF-6", "MDF 6 mm sheet", "MDF 6 مم", MaterialCategory.Sheet, MaterialUnit.Piece, 215m) : existingMaterials["MDF-6"];
        var mdf18 = existingMaterials is null ? Material("MDF-18", "MDF 18 mm sheet", "MDF 18 مم", MaterialCategory.Sheet, MaterialUnit.Piece, 450m) : existingMaterials["MDF-18"];
        var veneer = existingMaterials is null ? Material("VENEER-OAK", "Oak veneer with glue", "قشرة أرو بالغراء", MaterialCategory.Veneer, MaterialUnit.SquareMeter, 55m) : existingMaterials["VENEER-OAK"];
        var foam = existingMaterials is null ? Material("FOAM", "Foam", "فوم", MaterialCategory.Foam, MaterialUnit.Piece, 75m) : existingMaterials["FOAM"];
        var gasket = existingMaterials is null ? Material("WEATHER-GASKET", "Weather gasket", "جوان Weather gasket", MaterialCategory.Gasket, MaterialUnit.LinearMeter, 15m) : existingMaterials["WEATHER-GASKET"];
        var paint = existingMaterials is null ? Material("PAINT", "Paint materials", "خامات دهانات", MaterialCategory.Paint, MaterialUnit.SquareMeter, 75m) : existingMaterials["PAINT"];
        var packaging = existingMaterials is null ? Material("PACKAGING-CARDBOARD", "Packaging cardboard", "كارتون تغليف", MaterialCategory.Packaging, MaterialUnit.Kilogram, 22.5m) : existingMaterials["PACKAGING-CARDBOARD"];

        if (existingMaterials is null)
            dbContext.Materials.AddRange(mouski, oak, mdf6, mdf18, veneer, foam, gasket, paint, packaging);

        var template = existingTemplate ?? new DoorTemplate
        {
            Code = "HA-D04",
            NameEn = "Hassan Allam D04",
            NameAr = "حسن علام D04"
        };

        var version = new DoorTemplateVersion
        {
            DoorTemplate = template,
            Version = 1,
            EffectiveFromUtc = SourceEffectiveDateUtc,
            DefaultWidthMm = 970,
            DefaultHeightMm = 2200,
            FireRatingMinutes = 0,
            ChangeReason = "Initial version reconstructed from the 2022 cost-breakdown workbook.",
            SourceReference = "Break Dowen -10-2022.xlsx / 2200×970-علام"
        };

        var components = new[]
        {
            TimberRule("FRAME-V", "Frame vertical", "قائم الحلق", mouski, 10, 2m, DimensionSource.DoorHeight, 0m, null, 150m, 50m),
            TimberRule("FRAME-H", "Frame head", "رأس الحلق", mouski, 20, 1m, DimensionSource.DoorWidth, 0m, null, 150m, 50m),
            TimberRule("ARCH-V", "Architrave vertical", "قائم البر", mouski, 30, 4m, DimensionSource.DoorHeight, 80m, null, 75m, 25m),
            TimberRule("ARCH-H", "Architrave head", "رأس البر", mouski, 40, 2m, DimensionSource.DoorWidth, 160m, null, 75m, 25m),
            TimberRule("OAK-CASING-V", "Oak casing vertical", "قائم القشاط", oak, 50, 2m, DimensionSource.DoorHeight, -40m, null, 50m, 12.5m),
            TimberRule("OAK-CASING-H", "Oak casing head", "رأس القشاط", oak, 60, 2m, DimensionSource.DoorWidth, -70m, null, 50m, 12.5m),
            TimberRule("LEAF-STILE", "Leaf stile", "قائم شاسيه", mouski, 70, 2m, DimensionSource.DoorHeight, -40m, null, 125m, 38m),
            TimberRule("LEAF-RAIL", "Leaf rail", "رأس شاسيه", mouski, 80, 2m, DimensionSource.DoorWidth, -70m, null, 125m, 38m),
            TimberRule("CROSS-RAILS", "Cross rails", "السؤاسات", mouski, 90, 21m, DimensionSource.DoorWidth, -250m, null, 50m, 38m),
            TimberRule("FRAME-COMP-V", "Frame completion vertical", "قائم علفة كملة حلق", mouski, 100, 2m, DimensionSource.DoorHeight, 0m, null, 50m, 25m),
            TimberRule("FRAME-COMP-H", "Frame completion head", "راس علفة كملة حلق", mouski, 110, 1m, DimensionSource.DoorWidth, 0m, null, 50m, 25m),
            QuantityRule("MDF6", "MDF 6 mm", "ألواح MDF 6 مم", mdf6, 120, 2m),
            QuantityRule("MDF18", "MDF 18 mm", "MDF 18 مم", mdf18, 130, 0.5m),
            SurfaceRule("VENEER", "Oak veneer", "قشرة أرو بالغراء", veneer, 140, 1.35m),
            QuantityRule("FOAM", "Foam", "فوم", foam, 150, 1m),
            new DoorComponentRule
            {
                Code = "GASKET",
                NameEn = "Weather gasket",
                NameAr = "Weather gasket",
                Material = gasket,
                Sequence = 160,
                Formula = MeasurementFormula.ArchitravePerimeter,
                Quantity = 1m,
                MeasurementMultiplier = 1m,
                WastePercentage = 0m,
                LengthOffsetMm = -40m,
                WidthOffsetMm = -70m
            },
            SurfaceRule("PAINT", "Paint materials", "خامات دهانات", paint, 170, 1.20m),
            QuantityRule("PACKAGING", "Packaging cardboard", "كارتون تغليف", packaging, 180, 2.3m)
        };
        foreach (var component in components)
        {
            version.Components.Add(component);
        }

        template.Versions.Add(version);
        if (existingTemplate is not null)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        dbContext.DoorTemplates.Add(template);

        var profile = new PricingProfile
        {
            Code = "HA-2022",
            Name = "Hassan Allam 2022"
        };
        profile.Versions.Add(new PricingProfileVersion
        {
            Version = 1,
            EffectiveFromUtc = SourceEffectiveDateUtc,
            DefaultWastePercentage = 0m,
            AdministrativePercentage = 17.5m,
            ProfitPercentage = 25m,
            ManufacturingCost = 475m,
            TransportCost = 115m,
            InstallationCost = 475m,
            Currency = "EGP",
            ChangeReason = "Initial profile reconstructed from the 2022 workbook."
        });
        dbContext.PricingProfiles.Add(profile);

        var customer = new Customer { Code = "HASSAN-ALLAM", Name = "Hassan Allam" };
        customer.Projects.Add(new Project
        {
            Code = "SOURCE-2022",
            Name = "2022 source workbook reference"
        });
        dbContext.Customers.Add(customer);

        var currencySetting = new SystemSettingDefinition
        {
            Key = "pricing.default-currency",
            Name = "Default pricing currency",
            Description = "Default currency used by new pricing profiles.",
            ValueType = SettingValueType.Text,
            IsPricingSensitive = true
        };
        currencySetting.Versions.Add(new SystemSettingVersion
        {
            Version = 1,
            EffectiveFromUtc = SourceEffectiveDateUtc,
            Value = "EGP",
            ChangeReason = "Initial value from source workbook."
        });
        dbContext.SystemSettingDefinitions.Add(currencySetting);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static Material Material(
        string code,
        string nameEn,
        string nameAr,
        MaterialCategory category,
        MaterialUnit unit,
        decimal price)
    {
        var material = new Material
        {
            Code = code,
            NameEn = nameEn,
            NameAr = nameAr,
            Category = category,
            Unit = unit
        };
        material.Prices.Add(new MaterialPrice
        {
            Version = 1,
            EffectiveFromUtc = SourceEffectiveDateUtc,
            UnitPrice = price,
            Currency = "EGP",
            ChangeReason = "Initial price from source workbook."
        });
        return material;
    }

    private static DoorComponentRule TimberRule(
        string code,
        string nameEn,
        string nameAr,
        Material material,
        int sequence,
        decimal quantity,
        DimensionSource lengthSource,
        decimal lengthOffsetMm,
        decimal? fixedLengthMm,
        decimal widthMm,
        decimal thicknessMm) => new()
        {
            Code = code,
            NameEn = nameEn,
            NameAr = nameAr,
            Material = material,
            Sequence = sequence,
            Formula = MeasurementFormula.RectangularVolume,
            Quantity = quantity,
            MeasurementMultiplier = 1m,
            WastePercentage = 15m,
            LengthSource = lengthSource,
            LengthOffsetMm = lengthOffsetMm,
            FixedLengthMm = fixedLengthMm,
            WidthSource = DimensionSource.Fixed,
            FixedWidthMm = widthMm,
            ThicknessMm = thicknessMm
        };

    private static DoorComponentRule QuantityRule(
        string code,
        string nameEn,
        string nameAr,
        Material material,
        int sequence,
        decimal quantity) => new()
        {
            Code = code,
            NameEn = nameEn,
            NameAr = nameAr,
            Material = material,
            Sequence = sequence,
            Formula = MeasurementFormula.Quantity,
            Quantity = quantity,
            MeasurementMultiplier = 1m,
            WastePercentage = 0m
        };

    private static DoorComponentRule SurfaceRule(
        string code,
        string nameEn,
        string nameAr,
        Material material,
        int sequence,
        decimal multiplier) => new()
        {
            Code = code,
            NameEn = nameEn,
            NameAr = nameAr,
            Material = material,
            Sequence = sequence,
            Formula = MeasurementFormula.DoorSurfaceWithEdges,
            Quantity = 1m,
            MeasurementMultiplier = multiplier,
            WastePercentage = 0m,
            FixedWidthMm = 500m
        };
}
