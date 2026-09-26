using KarimDoors.Application.Lookups;

namespace KarimDoors.Application.Abstractions;

public interface IPricingLookupService
{
    Task<IReadOnlyList<PricingProjectOption>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoorTemplateOption>> GetDoorTemplatesAsync(CancellationToken cancellationToken = default);
}
