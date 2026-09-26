# Implementation status

## Implemented

- .NET 10 layered solution.
- Domain entities for materials, pricing history, door templates, pricing profiles, customers, projects, quotations, snapshots, settings and audit logs.
- Effective-date resolution for prices, door-template versions and pricing-profile versions.
- Deterministic pricing engine.
- Hassan Allam D04 spreadsheet regression case.
- SQLite development persistence and optional SQL Server provider.
- Seed data for the first spreadsheet validation case.
- Dashboard.
- Door price calculator.
- Detailed cost breakdown showing material price version per component.
- Material price catalog and full history.
- Add-material-price version workflow.
- Versioned system-settings UI.
- Basic audit log.
- Responsive light/dark custom UI.

## Deliberately not complete yet

- Authentication/authorization.
- Quotation CRUD and approval workflow.
- Door-template editor/version comparison UI.
- Pricing-profile editor/version comparison UI.
- Customer/project CRUD.
- EF migration set for production.
- Excel import workflow.
- Reporting / impact analysis.

These are the next features to implement after validating the initial pricing engine and data model.
