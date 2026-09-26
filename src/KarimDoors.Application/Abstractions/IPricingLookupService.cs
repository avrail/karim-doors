using KarimDoors.Application.Lookups;

namespace KarimDoors.Application.Abstractions;

public interface IPricingLookupService
{
    Task<IReadOnlyList<LookupOption>> GetDoorTemplatesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LookupOption>> GetPricingProfilesAsync(CancellationToken cancellationToken = default);
}
