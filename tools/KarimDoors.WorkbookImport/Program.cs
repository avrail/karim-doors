using System.Text.Json;
using KarimDoors.Application.Pricing;
using KarimDoors.Domain.Entities;
using KarimDoors.Domain.Enums;
using KarimDoors.Infrastructure.Persistence;
using KarimDoors.Infrastructure.Pricing;
using KarimDoors.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;

var path = args.Length > 0 ? args[0] : Path.Combine("docs", "source-data", "Break Dowen -10-2022.reference-cases.json");
var connection = Environment.GetEnvironmentVariable("ConnectionStrings__SqlServer");
if (string.IsNullOrWhiteSpace(connection))
    throw new InvalidOperationException("ConnectionStrings__SqlServer is required.");

using var document = JsonDocument.Parse(File.ReadAllText(path));
var options = new DbContextOptionsBuilder<KarimDoorsDbContext>().UseSqlServer(connection).Options;
await using var db = new KarimDoorsDbContext(options);
await db.Database.MigrateAsync();
await new DatabaseSeeder(db).SeedAsync();

var hassan = await db.Customers.SingleOrDefaultAsync(x => x.Code == "HASSAN-ALLAM")
    ?? throw new InvalidOperationException("The seeded Hassan Allam customer is required.");
var hassanProject = await db.Projects.SingleOrDefaultAsync(x => x.CustomerId == hassan.Id && x.Code == "SOURCE-2022")
    ?? throw new InvalidOperationException("The seeded Hassan Allam source project is required.");
if (hassanProject.Name == "2022 source workbook reference") hassanProject.Name = "Hassan Allam";
var redcon = await db.Customers.SingleOrDefaultAsync(x => x.Code == "REDCON");
if (redcon == null)
{
    redcon = new Customer { Code = "REDCON", Name = "Redcon" };
    db.Customers.Add(redcon);
}
var redconProject = redcon.Id == 0 ? null : await db.Projects.SingleOrDefaultAsync(x => x.CustomerId == redcon.Id && x.Code == "RED-2022");
if (redconProject == null)
{
    redconProject = new Project { Customer = redcon, Code = "RED-2022", Name = "Redcon" };
    db.Projects.Add(redconProject);
}
await db.SaveChangesAsync();
var projectIds = new Dictionary<string, int> { ["RED"] = redconProject.Id, ["HA"] = hassanProject.Id };
db.ChangeTracker.Clear();

var imported = 0;
var skipped = 0;
foreach (var item in document.RootElement.GetProperty("cases").EnumerateArray())
{
    var code = S(item, "code");
    if (await db.DoorTemplates.AnyAsync(x => x.Code == code))
    {
        var existing = await db.DoorTemplates.SingleAsync(x => x.Code == code);
        if (existing.ProjectId == null || existing.PricingProfileCode == null)
        {
            existing.ProjectId ??= projectIds[S(item, "customer")];
            existing.PricingProfileCode ??= code == "HA-D04" ? "HA-2022" : $"WB-{code}";
            existing.QuoteRoundingDigits = I(item, "quoteRoundingDigits");
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
        }
        var check = new PricingRequest(code, code == "HA-D04" ? "HA-2022" : $"WB-{code}",
            I(item, "widthMm"), I(item, "heightMm"), 1, DateTime.UtcNow);
        var calculated = new PricingEngine().Calculate(await new EfDoorPricingDataProvider(db).ResolveAsync(check), 1).UnitCalculatedPrice;
        var expected = D(item, "expectedCalculatedPrice");
        if (Math.Abs(calculated - expected) > 0.10m)
            throw new InvalidOperationException($"Existing {code} calculated {calculated}, workbook {expected}");
        Console.WriteLine($"VERIFIED {code}: {calculated:N2} EGP");
        Console.WriteLine($"SKIP {code}: already exists");
        skipped++;
        continue;
    }

    await using var transaction = await db.Database.BeginTransactionAsync();
    try
    {
        var date = DateTime.SpecifyKind(DateTime.Parse(S(item, "effectiveDate")), DateTimeKind.Utc);
        var customer = S(item, "customer");
        var profileCode = $"WB-{code}";
        var template = new DoorTemplate
        {
            Code = code,
            ProjectId = projectIds[customer],
            PricingProfileCode = profileCode,
            QuoteRoundingDigits = I(item, "quoteRoundingDigits"),
            NameEn = $"{customer} {code[(customer.Length + 1)..]}",
            NameAr = $"{(customer == "RED" ? "ريدكون" : "حسن علام")} {code[(customer.Length + 1)..]}",
            IsActive = true
        };
        var version = new DoorTemplateVersion
        {
            DoorTemplate = template,
            Version = 1,
            EffectiveFromUtc = date,
            DefaultWidthMm = I(item, "widthMm"),
            DefaultHeightMm = I(item, "heightMm"),
            FireRatingMinutes = I(item, "fireRatingMinutes"),
            ReferenceSizeOnly = true,
            SourceReference = $"Break Dowen -10-2022.xlsx / {S(item, "sourceSheet")}",
            ChangeReason = "Historical workbook reference case"
        };
        db.DoorTemplateVersions.Add(version);
        db.PricingProfileVersions.Add(new PricingProfileVersion
        {
            PricingProfile = new PricingProfile { Code = profileCode, Name = $"Workbook {code}", IsActive = true },
            Version = 1,
            EffectiveFromUtc = date,
            DefaultWastePercentage = 0,
            AdministrativePercentage = D(item, "administrativePercentage"),
            ProfitPercentage = D(item, "profitPercentage"),
            ManufacturingCost = D(item, "manufacturingCost"),
            TransportCost = D(item, "transportCost"),
            InstallationCost = D(item, "installationCost"),
            Currency = "EGP",
            ChangeReason = "Historical workbook reference case"
        });

        var sequence = 0;
        foreach (var component in item.GetProperty("components").EnumerateArray())
        {
            var material = new Material
            {
                Code = $"WB-{code}-{S(component, "code")}",
                NameEn = S(component, "nameEn"),
                NameAr = S(component, "nameAr"),
                Unit = Enum.Parse<MaterialUnit>(S(component, "unit")),
                Category = Category(S(component, "kind")),
                IsActive = true
            };
            db.MaterialPrices.Add(new MaterialPrice
            {
                Material = material,
                UnitPrice = D(component, "unitPrice"),
                EffectiveFromUtc = date,
                Version = 1,
                Currency = "EGP",
                ChangeReason = "Historical workbook reference case"
            });
            db.DoorComponentRules.Add(new DoorComponentRule
            {
                DoorTemplateVersion = version,
                Material = material,
                Code = S(component, "code"),
                NameEn = S(component, "nameEn"),
                NameAr = S(component, "nameAr"),
                Sequence = ++sequence,
                Formula = MeasurementFormula.FixedMeasurement,
                FixedMeasurement = D(component, "measurement"),
                Quantity = 1,
                MeasurementMultiplier = 1,
                WastePercentage = D(component, "wastePercentage")
            });
        }

        await db.SaveChangesAsync();
        var request = new PricingRequest(code, profileCode, version.DefaultWidthMm, version.DefaultHeightMm, 1, date.AddDays(1));
        var context = await new EfDoorPricingDataProvider(db).ResolveAsync(request);
        var actual = new PricingEngine().Calculate(context, 1).UnitCalculatedPrice;
        var expected = D(item, "expectedCalculatedPrice");
        if (Math.Abs(actual - expected) > 0.10m)
            throw new InvalidOperationException($"{code}: calculated {actual}, workbook {expected}");

        await transaction.CommitAsync();
        db.ChangeTracker.Clear();
        imported++;
        Console.WriteLine($"OK {code}: {actual:N2} EGP (workbook {expected:N2})");
    }
    catch
    {
        await transaction.RollbackAsync();
        db.ChangeTracker.Clear();
        throw;
    }
}

Console.WriteLine($"Imported {imported}; skipped {skipped}.");

// These models appear on quotation sheets, but the linked detail worksheets are absent.
// Keep them visible without making them selectable for pricing.
var pending = new (string Code, string Customer, int Width, int Fire)[]
{
    ("RED-D30", "Redcon", 0, 0),
    ("HA-D08-FD60-1000", "Hassan Allam", 1000, 60),
    ("HA-D08-FD60-1100", "Hassan Allam", 1100, 60),
    ("HA-D09-FD60-600", "Hassan Allam", 600, 60),
    ("HA-D10-FD90-1000", "Hassan Allam", 1000, 90),
    ("HA-D11-FD90-1100", "Hassan Allam", 1100, 90),
    ("HA-D14-FD90-1000", "Hassan Allam", 1000, 90)
};
foreach (var (code, customer, width, fire) in pending)
{
    var existingPending = await db.DoorTemplates.SingleOrDefaultAsync(x => x.Code == code);
    if (existingPending != null)
    {
        if (existingPending.ProjectId == null)
        {
            existingPending.ProjectId = projectIds[customer == "Redcon" ? "RED" : "HA"];
            await db.SaveChangesAsync();
        }
        continue;
    }
    var template = new DoorTemplate
    {
        Code = code,
        ProjectId = projectIds[customer == "Redcon" ? "RED" : "HA"],
        NameEn = $"{customer} {code.Split('-')[1]}",
        NameAr = $"{(customer == "Redcon" ? "ريدكون" : "حسن علام")} {code.Split('-')[1]}",
        IsActive = false
    };
    if (width > 0)
        template.Versions.Add(new DoorTemplateVersion
        {
            Version = 1,
            EffectiveFromUtc = new DateTime(2022, 10, 1, 0, 0, 0, DateTimeKind.Utc),
            DefaultWidthMm = width,
            DefaultHeightMm = 2200,
            FireRatingMinutes = fire,
            ReferenceSizeOnly = true,
            SourceReference = "Break Dowen -10-2022.xlsx / quotation only; linked detailed worksheet unavailable",
            ChangeReason = "Inactive pending cost breakdown"
        });
    db.DoorTemplates.Add(template);
}
await db.SaveChangesAsync();

static string S(JsonElement element, string name) => element.GetProperty(name).ToString();
static int I(JsonElement element, string name) => element.GetProperty(name).GetInt32();
static decimal D(JsonElement element, string name) => decimal.Parse(S(element, name), System.Globalization.CultureInfo.InvariantCulture);
static MaterialCategory Category(string kind) => kind switch
{
    "OAK" or "MOUSKI" => MaterialCategory.Timber,
    "VENEER" => MaterialCategory.Veneer,
    "FOAM" => MaterialCategory.Foam,
    "GASKET" => MaterialCategory.Gasket,
    "PAINT" => MaterialCategory.Paint,
    "PACKAGING" => MaterialCategory.Packaging,
    "FIRECORE" => MaterialCategory.FireCore,
    _ => MaterialCategory.Sheet
};
