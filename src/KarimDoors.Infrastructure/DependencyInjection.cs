using KarimDoors.Application.Abstractions;
using KarimDoors.Infrastructure.Persistence;
using KarimDoors.Infrastructure.Pricing;
using KarimDoors.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KarimDoors.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<KarimDoorsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("SqlServer");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("ConnectionStrings:SqlServer is required.");
            }

            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IDoorPricingDataProvider, EfDoorPricingDataProvider>();
        services.AddScoped<IMaterialHistoryService, MaterialHistoryService>();
        services.AddScoped<ISystemSettingsHistoryService, SystemSettingsHistoryService>();
        services.AddScoped<IPricingLookupService, PricingLookupService>();
        services.AddScoped<DatabaseSeeder>();
        return services;
    }
}
