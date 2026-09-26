# Versioning and History Strategy

## Requirement

A change to price, dimensions, waste, material selection, manufacturing cost, transport, installation, profit, administrative expense, or any other pricing input must never rewrite historical truth.

## Three complementary mechanisms

### 1. Effective-dated configuration
Used for business settings that evolve over time.

Typical fields:
- EffectiveFrom
- EffectiveTo
- VersionNumber
- ChangeReason
- CreatedOnUtc
- CreatedByUserId

Only one active version should apply for a given scope and effective date.

### 2. Audit trail
Records who changed what and when. Audit is operational traceability and is not a substitute for business versioning.

Audit entries should include:
- Entity type and identifier
- Action
- User
- Timestamp
- Old values
- New values
- Optional business reason

### 3. Immutable calculation snapshot
Every quotation revision stores the actual resolved inputs used by the pricing engine.

A snapshot must preserve at least:
- Door template/version
- Door dimensions
- Component rules and resolved dimensions
- Material identifiers, units, quantities, unit prices, and waste
- Manufacturing, transport, and installation costs
- Pricing profile/version
- Expense and profit rules
- Intermediate totals and final calculated price
- Any manual commercial override and its reason

## Quotation revisions

A quotation is a business identity. Changes after issue/approval create a new revision rather than modifying the old revision.

Example:
- Q-2026-00128 Rev 0
- Q-2026-00128 Rev 1
- Q-2026-00128 Rev 2

Approved, sent, accepted, rejected, and cancelled revisions remain immutable.

## Historical calculation

The system should support two different questions:

1. "Show me exactly how quotation Q was calculated." -> use its immutable snapshot.
2. "What would this door have cost on date D?" -> resolve effective-dated configuration valid on D and run the pricing engine.
