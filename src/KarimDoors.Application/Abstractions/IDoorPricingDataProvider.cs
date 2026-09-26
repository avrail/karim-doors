using KarimDoors.Application.Pricing;

namespace KarimDoors.Application.Abstractions;

public interface IDoorPricingDataProvider
{
    Task<PricingContext> ResolveAsync(
        PricingRequest request,
        CancellationToken cancellationToken = default);
}
