using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Pricing;
using Microsoft.Extensions.DependencyInjection;

namespace KarimDoors.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PricingEngine>();
        services.AddScoped<IDoorPricingService, DoorPricingService>();
        return services;
    }
}
