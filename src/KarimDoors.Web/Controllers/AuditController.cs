using KarimDoors.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Web.Controllers;

public sealed class AuditController(KarimDoorsDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await dbContext.AuditLogs
            .AsNoTracking()
            .OrderByDescending(x => x.OccurredOnUtc)
            .Take(200)
            .ToListAsync(cancellationToken);
        return View(items);
    }
}
