using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Lookups;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Infrastructure.Persistence;

public sealed class PricingLookupService(KarimDoorsDbContext dbContext) : IPricingLookupService
{
    public async Task<IReadOnlyList<LookupOption>> GetDoorTemplatesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.DoorTemplates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.NameEn)
            .Select(x => new LookupOption(x.Code, $"{x.Code} · {x.NameEn}"))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LookupOption>> GetPricingProfilesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.PricingProfiles
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new LookupOption(x.Code, $"{x.Code} · {x.Name}"))
            .ToListAsync(cancellationToken);
}
