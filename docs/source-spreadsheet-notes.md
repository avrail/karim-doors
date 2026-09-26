# Source Spreadsheet Notes

The existing breakdown workbook is used to discover business rules and to provide regression examples for the future pricing engine.

## What it represents

The workbook combines:
- Door dimensions
- Timber and other material quantities
- Component-level sizing rules
- Waste
- Sheet/veneer/foam/fire-core costs
- Paint/finishing
- Manufacturing
- Transport
- Installation
- Administrative/engineering expenses
- Profit/markup
- Final quotation values

## Migration principle

The application will not copy Excel cell formulas directly into the database. Each supported pricing rule will be represented explicitly as structured domain data.

## Known source-data concerns

The workbook contains historical values and should not be interpreted as current market pricing. It also contains external workbook references and broken references in some areas. These must not be carried into the runtime model.

## Validation strategy

Before production use, representative doors from the workbook will become automated regression cases. The .NET pricing engine should reproduce the workbook's expected historical calculations within an agreed rounding tolerance.

## Data privacy

The source workbook is intentionally excluded from Git because this repository is public and the workbook contains business pricing data. Keep it locally under `docs/source-data/` if needed for development.
