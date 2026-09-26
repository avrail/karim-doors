namespace KarimDoors.Application.Pricing;

public sealed record PricingProfileInput(
    string Code,
    int Version,
    decimal AdministrativePercentage,
    decimal ProfitPercentage,
    decimal ManufacturingCost,
    decimal TransportCost,
    decimal InstallationCost,
    string Currency);
