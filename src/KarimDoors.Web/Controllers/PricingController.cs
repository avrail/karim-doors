using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Pricing;
using KarimDoors.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace KarimDoors.Web.Controllers;

public sealed class PricingController(
    IDoorPricingService pricingService,
    IPricingLookupService lookupService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Calculator(CancellationToken cancellationToken)
    {
        var model = new PricingCalculatorViewModel();
        await PopulateLookupsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Calculator(
        PricingCalculatorViewModel model,
        CancellationToken cancellationToken)
    {
        await PopulateLookupsAsync(model, cancellationToken);

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = new PricingRequest(
                model.DoorTemplateCode,
                model.PricingProfileCode,
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

    private async Task PopulateLookupsAsync(
        PricingCalculatorViewModel model,
        CancellationToken cancellationToken)
    {
        model.DoorTemplates = await lookupService.GetDoorTemplatesAsync(cancellationToken);
        model.PricingProfiles = await lookupService.GetPricingProfilesAsync(cancellationToken);
    }
}
