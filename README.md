# Karim Doors

Karim Doors is a .NET 10 door costing and quotation system designed to replace spreadsheet-based pricing with a traceable, versioned pricing engine.

## Core goals

- Calculate door prices from dimensions, components, material quantities, waste, manufacturing, transport, installation, expenses, and profit rules.
- Preserve full history whenever prices, dimensions, formulas, or pricing settings change.
- Keep approved quotations immutable through calculation snapshots and quotation revisions.
- Support effective-dated configuration so historical prices can be reproduced accurately.
- Provide a professional modern interface for pricing, breakdown analysis, history, and quotations.

## Technology direction

- ASP.NET Core 10 MVC
- .NET 10
- EF Core 10
- SQL Server
- Razor Views
- Automated pricing-engine tests

## Solution structure

```text
src/
  KarimDoors.Domain/
  KarimDoors.Application/
  KarimDoors.Infrastructure/
  KarimDoors.Web/

docs/
  architecture.md
  database-design.md
  pricing-rules.md
  versioning-strategy.md
  source-spreadsheet-notes.md
```

## Fundamental rule

Anything that can affect a price is treated as versioned business configuration. Historical quotations store immutable calculation snapshots so later changes never alter old results.

The original spreadsheet is a business-rules reference and validation source, not the runtime pricing engine.
