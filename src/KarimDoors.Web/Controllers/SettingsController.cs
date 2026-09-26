using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Settings;
using KarimDoors.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace KarimDoors.Web.Controllers;

public sealed class SettingsController(ISystemSettingsHistoryService settingsService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var settings = await settingsService.GetSettingsAsync(DateTime.UtcNow, cancellationToken);
        return View(settings);
    }

    public async Task<IActionResult> History(int id, CancellationToken cancellationToken)
    {
        var setting = await settingsService.GetHistoryAsync(id, cancellationToken);
        return setting is null ? NotFound() : View(setting);
    }

    [HttpGet]
    public async Task<IActionResult> AddVersion(int id, CancellationToken cancellationToken)
    {
        var setting = await settingsService.GetHistoryAsync(id, cancellationToken);
        if (setting is null)
            return NotFound();

        return View(new AddSettingVersionViewModel
        {
            SettingDefinitionId = id,
            SettingName = setting.Name,
            Value = setting.Versions.FirstOrDefault()?.Value ?? string.Empty,
            EffectiveFrom = DateTime.UtcNow.Date
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddVersion(
        AddSettingVersionViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await settingsService.AddVersionAsync(
                new AddSettingVersion(
                    model.SettingDefinitionId,
                    model.Value,
                    DateTime.SpecifyKind(model.EffectiveFrom.Date, DateTimeKind.Utc),
                    model.ChangeReason,
                    User.Identity?.Name ?? "local-user"),
                cancellationToken);

            TempData["Success"] = "A new setting version was created. The previous value remains in history.";
            return RedirectToAction(nameof(History), new { id = model.SettingDefinitionId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}
