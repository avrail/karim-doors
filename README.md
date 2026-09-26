# Karim Doors

Karim Doors is a .NET 10 door costing and quotation platform that replaces spreadsheet-based pricing with a traceable, effective-dated pricing engine.

The project was designed around one non-negotiable rule:

> Anything that can affect a price must keep its history. Existing quotations must never change because a material price, dimension rule, margin, or system setting changed later.

## What is implemented in this MVP

- ASP.NET Core 10 MVC application.
- EF Core 10 persistence layer.
- SQL Server database with EF Core migrations.
- Effective-dated material price history.
- Versioned door templates and component rules.
- Versioned pricing profiles.
- Generic versioned system settings.
- Pure pricing engine that calculates from dimensions, component formulas, waste, material prices, manufacturing, transport, installation, administrative cost, and profit.
- Professional responsive UI with light/dark mode.
- Material price history screen and new-price-version workflow.
- System setting history screen and version workflow.
- Audit events for material-price and system-setting changes.
- Quotation / revision / calculation-snapshot domain model ready for the quotation workflow.
- Automated pricing regression test based on the source Excel workbook.

## Excel validation case

The initial seed contains the Hassan Allam D04 example reconstructed from:

`Break Dowen -10-2022.xlsx / 2200×970-علام`

Reference calculation:

| Metric | Expected |
| --- | ---: |
| Size | 970 × 2200 mm |
| Material cost | 3,496.84 EGP |
| Manufacturing | 475.00 EGP |
| Transport | 115.00 EGP |
| Installation | 475.00 EGP |
| Dry cost | 4,561.84 EGP |
| Administrative cost (17.5%) | 798.32 EGP |
| Profit (25%) | 1,140.46 EGP |
| Calculated price | **6,500.63 EGP** |

The regression test verifies the .NET pricing engine against these values.

## Solution structure

```text
KarimDoors.sln

src/
  KarimDoors.Domain/
      Common/
      Entities/
      Enums/

  KarimDoors.Application/
      Abstractions/
      Pricing/
      Materials/
      Settings/
      Lookups/

  KarimDoors.Infrastructure/
      Persistence/
      Pricing/
      Seeding/

  KarimDoors.Web/
      Controllers/
      Models/
      Views/
      wwwroot/

tests/
  KarimDoors.Application.Tests/

docs/
  architecture.md
  database-design.md
  pricing-rules.md
  versioning-strategy.md
  source-spreadsheet-notes.md
```

## Run locally

The interface supports English and Arabic. Use the language link in the top bar to switch; the choice is saved in a cookie. Arabic uses a right-to-left layout. Pricing values and form numbers continue to use the existing decimal format.

Requirements:

- .NET 10 SDK

From the repository root:

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project .\src\KarimDoors.Web\KarimDoors.Web.csproj
```

Development uses the `KarimDoors` database on `ADB10CIC0W98361\MSSQLSERVER01` with Windows authentication and `TrustServerCertificate=True`. It applies migrations and seeds sample door data when started. The Windows account running the app needs access to that database.

## Production SQL Server

The base configuration connects to the named SQL Server instance with Windows authentication. Set `ConnectionStrings__SqlServer` through the deployment environment or a secret store to override it for another application host or database. The application does not create or migrate a production database at startup. Apply the EF Core migration before starting it. For example, with Windows authentication on the application host:

```powershell
$env:ConnectionStrings__SqlServer = "Server=ADB10CIC0W98361\MSSQLSERVER01;Database=KarimDoors;Integrated Security=True;TrustServerCertificate=True"
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/KarimDoors.Infrastructure --startup-project src/KarimDoors.Web
```

The named SQL Server instance and database name in this example are deployment-specific. Use a dedicated SQL login if the application host cannot use Windows authentication. Store its password in the deployment secret store, never in Git. The initial migration creates an empty schema; the spreadsheet-based sample data is seeded only in Development.

## History model

### Material price changes

A material price update does not overwrite the previous row.

Example:

```text
Mouski timber
v1  9,500 EGP/m³   22-Jan-2022 → 30-Sep-2026
v2  37,500 EGP/m³  01-Oct-2026 → Current
```

### Door configuration changes

Door template versions are effective-dated. A future change to frame thickness, quantity, waste, or formula should create a new `DoorTemplateVersion` rather than editing a historical version.

### Quotations

The domain already contains:

- `Quotation`
- `QuotationRevision`
- `QuotationItem`
- `CalculationSnapshot`

The intended workflow is that an approved quotation revision becomes immutable and stores the full calculation input, breakdown, and configuration snapshot.

## Source spreadsheet

The Excel workbook is intentionally excluded from Git by `.gitignore` because the repository is public and the workbook contains business pricing data.

Keep it locally under:

```text
docs/source-data/Break Dowen -10-2022.xlsx
```

The spreadsheet is a validation/reference source only. Runtime pricing must come from the database and the pricing engine.

## Next implementation milestones

1. Door-template management UI with version comparison.
2. Pricing-profile version management UI.
3. Customer and project management.
4. Quotation creation, revisioning, approval, commercial price override, and calculation snapshots.
5. Authentication and role-based permissions.
6. EF Core migrations and production SQL Server deployment.
7. Reports and price-change impact analysis.
