using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Lookups;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Infrastructure.Persistence;

public sealed class PricingLookupService(KarimDoorsDbContext dbContext) : IPricingLookupService
{
    public async Task<IReadOnlyList<PricingProjectOption>> GetProjectsAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Projects.AsNoTracking()
            .Where(p => p.IsActive && dbContext.DoorTemplates.Any(d => d.IsActive && d.ProjectId == p.Id))
            .OrderBy(p => p.Name)
            .Select(p => new PricingProjectOption(p.Id, p.Code, p.Name))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<DoorTemplateOption>> GetDoorTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var doors = await dbContext.DoorTemplates.AsNoTracking()
            .Where(x => x.IsActive && x.ProjectId != null)
            .Include(x => x.Versions)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
        return doors.Select(x =>
        {
            var version = x.Versions.OrderByDescending(v => v.Version).First();
            return new DoorTemplateOption(x.Code, x.NameEn, x.NameAr, x.ProjectId!.Value,
                version.DefaultWidthMm, version.DefaultHeightMm, version.ReferenceSizeOnly);
        }).ToList();
    }

}
