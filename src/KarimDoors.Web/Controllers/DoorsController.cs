using KarimDoors.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Web.Controllers;

public sealed class DoorsController(KarimDoorsDbContext db) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var doors = await db.DoorTemplates.AsNoTracking().Include(x => x.Project).Include(x => x.Versions)
            .OrderBy(x => x.Code).ToListAsync(ct);
        return View(doors);
    }
}
