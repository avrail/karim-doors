using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Pricing;
using KarimDoors.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KarimDoors.Web.Controllers;

public sealed class PricingController(
    IDoorPricingService pricingService,
    IPricingLookupService lookupService,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Calculator(CancellationToken cancellationToken)
    {
        var model = new PricingCalculatorViewModel();
        await PopulateLookupsAsync(model, cancellationToken);
        var selected = model.DoorTemplates.FirstOrDefault(x => x.Code == model.DoorTemplateCode)
            ?? model.DoorTemplates.FirstOrDefault();
        if (selected is not null)
        {
            model.ProjectId = selected.ProjectId;
            model.DoorTemplateCode = selected.Code;
            model.WidthMm = selected.WidthMm;
            model.HeightMm = selected.HeightMm;
        }
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Calculator(PricingCalculatorViewModel model, CancellationToken cancellationToken)
    {
        await PopulateLookupsAsync(model, cancellationToken);
        var selected = model.DoorTemplates.FirstOrDefault(x => x.Code == model.DoorTemplateCode);
        if (selected is null || selected.ProjectId != model.ProjectId ||
            !model.Projects.Any(x => x.Id == model.ProjectId))
            ModelState.AddModelError(nameof(model.DoorTemplateCode), localizer["Select a door from the chosen project."]);
        if (!ModelState.IsValid) return View(model);

        try
        {
            // Historical workbook cases have one matching pricing profile per door.
            var profileCode = model.DoorTemplateCode == "HA-D04"
                ? "HA-2022" : $"WB-{model.DoorTemplateCode}";
            var request = new PricingRequest(
                model.DoorTemplateCode,
                profileCode,
                model.WidthMm,
                model.HeightMm,
                model.Quantity,
                DateTime.SpecifyKind(model.CalculationDate.Date, DateTimeKind.Utc));
            model.Result = await pricingService.CalculateAsync(request, cancellationToken);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            model.ErrorMessage = ex.Message;
        }
        return View(model);
    }

    private async Task PopulateLookupsAsync(PricingCalculatorViewModel model, CancellationToken cancellationToken)
    {
        model.Projects = await lookupService.GetProjectsAsync(cancellationToken);
        model.DoorTemplates = await lookupService.GetDoorTemplatesAsync(cancellationToken);
    }
}
