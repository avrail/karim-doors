namespace KarimDoors.Application.Pricing;

public sealed record PricingContext(
    string DoorTemplateCode,
    string DoorTemplateName,
    int DoorTemplateVersion,
    int FireRatingMinutes,
    int WidthMm,
    int HeightMm,
    PricingProfileInput Profile,
    IReadOnlyList<PricingComponentInput> Components,
    DateTime CalculationDateUtc);
