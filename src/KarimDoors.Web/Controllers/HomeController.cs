using KarimDoors.Infrastructure.Persistence;
using KarimDoors.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Web.Controllers;

public sealed class HomeController(KarimDoorsDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new DashboardViewModel
        {
            MaterialsCount = await dbContext.Materials.CountAsync(cancellationToken),
            DoorTemplatesCount = await dbContext.DoorTemplates.CountAsync(cancellationToken),
            PricingProfilesCount = await dbContext.PricingProfiles.CountAsync(cancellationToken),
            QuotationsCount = await dbContext.Quotations.CountAsync(cancellationToken),
            AuditEventsCount = await dbContext.AuditLogs.CountAsync(cancellationToken),
            GeneratedOnUtc = DateTime.UtcNow
        };
        return View(model);
    }

    [Route("Home/Error")]
    public IActionResult Error() => View();
}
