using KarimDoors.Application.Pricing;

namespace KarimDoors.Application.Abstractions;

public interface IDoorPricingService
{
    Task<PricingResult> CalculateAsync(
        PricingRequest request,
        CancellationToken cancellationToken = default);
}
