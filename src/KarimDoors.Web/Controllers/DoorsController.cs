using System.Text.Json;
using System.Text.RegularExpressions;
using KarimDoors.Domain.Entities;
using KarimDoors.Domain.Enums;
using KarimDoors.Infrastructure.Persistence;
using KarimDoors.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace KarimDoors.Web.Controllers;

public sealed class DoorsController(KarimDoorsDbContext db, IStringLocalizer<SharedResource> localizer) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var doors = await db.DoorTemplates.AsNoTracking().Include(x => x.Project).Include(x => x.Versions)
            .OrderBy(x => x.Code).ToListAsync(ct);
        return View(doors);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var model = new CreateDoorViewModel();
        await PopulateProjects(model, ct);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDoorViewModel model, CancellationToken ct)
    {
        await PopulateProjects(model, ct);
        if (!model.Projects.Any(x => x.Id == model.ProjectId))
            ModelState.AddModelError(nameof(model.ProjectId), localizer["Select an active project."]);

        var code = model.Code?.Trim().ToUpperInvariant() ?? string.Empty;
        if (await db.DoorTemplates.AnyAsync(x => x.Code == code, ct))
            ModelState.AddModelError(nameof(model.Code), localizer["This door code already exists."]);

        var components = (model.Components ?? [])
            .Select((value, index) => (Value: value, Index: index))
            .Where(x => !string.IsNullOrWhiteSpace(x.Value.Code) ||
                !string.IsNullOrWhiteSpace(x.Value.NameEn) || !string.IsNullOrWhiteSpace(x.Value.NameAr) ||
                x.Value.Measurement != 0 || x.Value.UnitPrice != 0)
            .ToList();
        if (components.Count == 0)
            ModelState.AddModelError(nameof(model.Components), localizer["Add at least one cost component."]);

        var componentCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (value, index) in components)
        {
            var componentCode = value.Code?.Trim().ToUpperInvariant() ?? string.Empty;
            if (!Regex.IsMatch(componentCode, "^[A-Z0-9][A-Z0-9-]{0,14}$") || !componentCodes.Add(componentCode))
                ModelState.AddModelError($"Components[{index}].Code", localizer["Each component needs a unique code of up to 15 letters, numbers, or hyphens."]);
            if (string.IsNullOrWhiteSpace(value.NameEn) || value.NameEn.Length > 200 ||
                string.IsNullOrWhiteSpace(value.NameAr) || value.NameAr.Length > 200)
                ModelState.AddModelError($"Components[{index}].NameEn", localizer["Enter both component names (up to 200 characters)."]);
            if (!Enum.IsDefined(value.Category) || !Enum.IsDefined(value.Unit))
                ModelState.AddModelError($"Components[{index}].Unit", localizer["Choose a valid category and unit."]);
            if (value.Measurement <= 0 || value.Measurement > 999999m ||
                decimal.Round(value.Measurement, 6) != value.Measurement)
                ModelState.AddModelError($"Components[{index}].Measurement", localizer["Enter a positive measurement with at most six decimal places."]);
            if (value.UnitPrice <= 0 || value.UnitPrice > 999999999m ||
                decimal.Round(value.UnitPrice, 4) != value.UnitPrice)
                ModelState.AddModelError($"Components[{index}].UnitPrice", localizer["Enter a positive unit price with at most four decimal places."]);
            if (value.WastePercentage < 0 || value.WastePercentage > 100)
                ModelState.AddModelError($"Components[{index}].WastePercentage", localizer["Waste must be between 0 and 100 percent."]);
        }
        if (!ModelState.IsValid) return View(model);

        // The calculator accepts a date (without time), so today's prices start at UTC midnight.
        var now = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
        var profileCode = $"DOOR-{code}";
        var template = new DoorTemplate
        {
            ProjectId = model.ProjectId,
            Code = code,
            NameEn = model.NameEn.Trim(),
            NameAr = model.NameAr.Trim(),
            PricingProfileCode = profileCode,
            QuoteRoundingDigits = model.QuoteRoundingDigits,
            IsActive = true
        };
        var version = new DoorTemplateVersion
        {
            DoorTemplate = template,
            Version = 1,
            EffectiveFromUtc = now,
            DefaultWidthMm = model.WidthMm,
            DefaultHeightMm = model.HeightMm,
            FireRatingMinutes = model.FireRatingMinutes,
            ReferenceSizeOnly = true,
            ChangeReason = "Initial manually entered cost breakdown"
        };
        foreach (var (value, index) in components)
        {
            var componentCode = value.Code!.Trim().ToUpperInvariant();
            var material = new Material
            {
                Code = $"M-{code}-{componentCode}",
                NameEn = value.NameEn!.Trim(),
                NameAr = value.NameAr!.Trim(),
                Category = value.Category,
                Unit = value.Unit
            };
            material.Prices.Add(new MaterialPrice
            {
                UnitPrice = value.UnitPrice,
                Currency = "EGP",
                Version = 1,
                EffectiveFromUtc = now,
                ChangeReason = "Initial price for manually created door"
            });
            version.Components.Add(new DoorComponentRule
            {
                Material = material,
                Code = componentCode,
                NameEn = material.NameEn,
                NameAr = material.NameAr,
                Sequence = index + 1,
                Formula = MeasurementFormula.FixedMeasurement,
                FixedMeasurement = value.Measurement,
                Quantity = 1,
                MeasurementMultiplier = 1,
                WastePercentage = value.WastePercentage
            });
        }
        db.DoorTemplateVersions.Add(version);
        db.PricingProfileVersions.Add(new PricingProfileVersion
        {
            PricingProfile = new PricingProfile { Code = profileCode, Name = $"{code} pricing" },
            Version = 1,
            EffectiveFromUtc = now,
            ManufacturingCost = model.ManufacturingCost,
            TransportCost = model.TransportCost,
            InstallationCost = model.InstallationCost,
            AdministrativePercentage = model.AdministrativePercentage,
            ProfitPercentage = model.ProfitPercentage,
            DefaultWastePercentage = 0,
            Currency = "EGP",
            ChangeReason = "Initial profile for manually created door"
        });
        db.AuditLogs.Add(new AuditLog
        {
            EntityName = nameof(DoorTemplate), EntityKey = code, Action = "Create",
            NewValuesJson = JsonSerializer.Serialize(new { model.ProjectId, code, model.NameEn, model.NameAr,
                model.WidthMm, model.HeightMm, model.FireRatingMinutes, model.QuoteRoundingDigits,
                model.ManufacturingCost, model.TransportCost, model.InstallationCost,
                model.AdministrativePercentage, model.ProfitPercentage, Components = components.Select(x => x.Value) }),
            Reason = "New priced door model"
        });
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            ModelState.AddModelError(nameof(model.Code), localizer["This door code already exists."]);
            return View(model);
        }
        TempData["Success"] = "Door model and pricing saved.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateProjects(CreateDoorViewModel model, CancellationToken ct) =>
        model.Projects = await db.Projects.AsNoTracking().Where(x => x.IsActive)
            .OrderBy(x => x.Name).ToListAsync(ct);
}
