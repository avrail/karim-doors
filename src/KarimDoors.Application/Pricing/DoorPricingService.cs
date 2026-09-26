using KarimDoors.Application.Abstractions;

namespace KarimDoors.Application.Pricing;

public sealed class DoorPricingService(
    IDoorPricingDataProvider dataProvider,
    PricingEngine pricingEngine) : IDoorPricingService
{
    public async Task<PricingResult> CalculateAsync(
        PricingRequest request,
        CancellationToken cancellationToken = default)
    {
        var context = await dataProvider.ResolveAsync(request, cancellationToken);
        return pricingEngine.Calculate(context, request.Quantity);
    }
}
