# Pricing Rules

## High-level calculation

The spreadsheet indicates a pricing flow broadly equivalent to:

1. Resolve door template and component rules.
2. Resolve component dimensions from door width/height and offsets.
3. Calculate material quantity/volume/area as appropriate.
4. Apply waste where applicable.
5. Resolve effective material price for the calculation date.
6. Calculate material cost.
7. Add manufacturing cost.
8. Add transport cost.
9. Add installation cost.
10. Calculate dry cost.
11. Apply administrative/engineering expenses.
12. Apply profit/markup according to the active pricing profile.
13. Produce calculated selling price.
14. Optionally allow an authorized commercial override, preserving both values.

## Example dimensional rules observed in the source model

Rules vary by door/template and therefore must be data-driven. Examples include:
- Component length = DoorHeight
- Component length = DoorHeight + offset
- Component length = DoorWidth + offset
- Component length = DoorHeight - offset
- Component length = DoorWidth - offset

The application must not store raw Excel expressions such as `F4+0.08`. It should represent supported rules as structured data, for example:

- DimensionSource = DoorHeight
- Operation = Add
- Offset = 0.08 m

## Important configurable percentages

Observed source workbooks contain differing values for items such as:
- Waste percentage
- Administrative/engineering expenses
- Profit/markup

These values must be configuration/version data, never hard-coded constants.

## Calculated vs quoted price

The model must distinguish:
- CalculatedPrice
- SuggestedSellingPrice
- OverrideSellingPrice
- FinalSellingPrice

A manual override requires authorization and a reason and must be included in the quotation snapshot/audit trail.
