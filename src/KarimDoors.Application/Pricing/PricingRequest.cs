namespace KarimDoors.Application.Pricing;

public sealed record PricingRequest(
    string DoorTemplateCode,
    string PricingProfileCode,
    int WidthMm,
    int HeightMm,
    decimal Quantity,
    DateTime CalculationDateUtc);
