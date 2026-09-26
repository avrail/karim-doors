# Architecture

## Objective

Build a maintainable .NET 10 system that calculates door cost and selling price while preserving the exact historical configuration used for every quotation.

## Layers

### KarimDoors.Domain
Contains business concepts and invariants only. It must not depend on EF Core, MVC, SQL Server, or UI concerns.

Planned areas:
- Materials and units
- Door templates and components
- Effective-dated versions
- Pricing profiles
- Customers and projects
- Quotations and revisions
- Calculation snapshots

### KarimDoors.Application
Contains use cases, pricing orchestration, DTOs, validation contracts, and service interfaces.

### KarimDoors.Infrastructure
Contains EF Core, SQL Server mappings, persistence, audit persistence, and external integrations.

### KarimDoors.Web
ASP.NET Core MVC/Razor UI. The visual design will use a dedicated design system rather than default Bootstrap styling.

## Key architectural principles

1. Historical correctness is more important than convenience.
2. Business configuration that affects price is effective-dated and versioned.
3. Approved quotation revisions are immutable.
4. Calculations create snapshots of all inputs used.
5. Current master data and historical transactional data are separated.
6. The pricing engine is independent from MVC and EF Core so it can be unit tested.
