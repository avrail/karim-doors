# Database Design - First Blueprint

This is a conceptual starting point. Detailed entity fields and EF Core mappings will be finalized before the first migration.

## Master data

### Material
Stable identity and descriptive data for a material.

### MaterialPriceVersion
Effective-dated material price history.

### UnitOfMeasure
Examples: m, m2, m3, piece, kg, litre.

### DoorTemplate
Stable identity for a door design/model.

### DoorTemplateVersion
Effective-dated version of a door template.

### DoorComponentDefinition
Logical component such as frame stile, head, leaf stile, rail, sheet, veneer, foam, paint, packaging, hardware, or fire core.

### DoorComponentRuleVersion
Versioned quantity/dimension/material/waste rule used by a template version.

### FireRating
Examples: Non-Fire, FD30, FD60, FD90.

### PricingProfile
Stable pricing-policy identity, optionally scoped by customer/project.

### PricingProfileVersion
Effective-dated values for expenses, profit, default waste, manufacturing, transport, installation, or other commercial rules.

## Commercial data

### Customer
Customer identity.

### Project
Customer project and optional project-specific pricing configuration.

### Quotation
Stable quotation identity/number.

### QuotationRevision
Immutable business revision after issue/approval.

### QuotationItem
Door/configuration line within a quotation revision.

### CalculationSnapshot
Immutable calculation header with the exact versions and totals used.

### CalculationSnapshotItem
Immutable component/material-level breakdown.

## Governance

### AuditLog
Operational audit trail for create/update/delete/status/override actions.

### ChangeReason
May be modeled explicitly or captured consistently on versioned records depending on the final domain model.

## Constraints to enforce

- No overlapping effective-date ranges for the same material price scope.
- No overlapping active pricing-profile versions for the same scope.
- No overlapping door-template versions for the same template/effective period.
- Approved quotation revisions cannot be edited.
- Snapshot records cannot be updated after creation except by controlled administrative repair procedures.
