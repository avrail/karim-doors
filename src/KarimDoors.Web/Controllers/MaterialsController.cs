using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Materials;
using KarimDoors.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace KarimDoors.Web.Controllers;

public sealed class MaterialsController(IMaterialHistoryService materialHistoryService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var materials = await materialHistoryService.GetMaterialsAsync(DateTime.UtcNow, cancellationToken);
        return View(materials);
    }

    public async Task<IActionResult> History(int id, CancellationToken cancellationToken)
    {
        var material = await materialHistoryService.GetHistoryAsync(id, cancellationToken);
        return material is null ? NotFound() : View(material);
    }

    [HttpGet]
    public async Task<IActionResult> AddPrice(int id, CancellationToken cancellationToken)
    {
        var material = await materialHistoryService.GetHistoryAsync(id, cancellationToken);
        if (material is null)
            return NotFound();

        return View(new AddMaterialPriceViewModel
        {
            MaterialId = id,
            MaterialName = $"{material.Code} · {material.NameEn}",
            Currency = material.Prices.FirstOrDefault()?.Currency ?? "EGP",
            EffectiveFrom = DateTime.UtcNow.Date
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPrice(
        AddMaterialPriceViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await materialHistoryService.AddPriceVersionAsync(
                new AddMaterialPriceVersion(
                    model.MaterialId,
                    model.UnitPrice,
                    model.Currency,
                    DateTime.SpecifyKind(model.EffectiveFrom.Date, DateTimeKind.Utc),
                    model.ChangeReason,
                    model.SupplierReference,
                    User.Identity?.Name ?? "local-user"),
                cancellationToken);

            TempData["Success"] = "A new price version was created. Historical versions were preserved.";
            return RedirectToAction(nameof(History), new { id = model.MaterialId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}
