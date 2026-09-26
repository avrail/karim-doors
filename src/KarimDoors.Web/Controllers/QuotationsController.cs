using System.Text.Json;
using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Pricing;
using KarimDoors.Domain.Entities;
using KarimDoors.Infrastructure.Persistence;
using KarimDoors.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Web.Controllers;

public sealed class QuotationsController(KarimDoorsDbContext db, IDoorPricingService pricing) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct) => View(await db.Quotations
        .AsNoTracking().Include(x => x.Customer).Include(x => x.Project)
        .OrderByDescending(x => x.Id).Take(100).ToListAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var model = new QuotationDraftViewModel { Lines = [new(), new(), new(), new(), new()] };
        await Populate(model, ct);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuotationDraftViewModel model, CancellationToken ct)
    {
        await Populate(model, ct);
        var selected = model.Lines.Where(x => !string.IsNullOrWhiteSpace(x.DoorCode)).ToList();
        if (selected.Count == 0) ModelState.AddModelError(nameof(model.Lines), "Select at least one door.");
        if (selected.Any(x => x.Quantity <= 0)) ModelState.AddModelError(nameof(model.Lines), "Quantity must be positive.");
        if (!ModelState.IsValid) return View(model);

        var date = DateTime.UtcNow;
        var results = new List<(DoorTemplate Door, QuotationDraftLine Line, PricingResult Price)>();
        try
        {
            foreach (var line in selected)
            {
                var door = await db.DoorTemplates.Include(x => x.Versions)
                    .SingleOrDefaultAsync(x => x.Code == line.DoorCode && x.IsActive, ct)
                    ?? throw new InvalidOperationException($"Door {line.DoorCode} is unavailable.");
                var version = door.Versions.Where(x => x.EffectiveFromUtc <= date && (x.EffectiveToUtc == null || x.EffectiveToUtc > date))
                    .OrderByDescending(x => x.Version).FirstOrDefault()
                    ?? throw new InvalidOperationException($"Door {line.DoorCode} has no current price.");
                var profile = line.DoorCode == "HA-D04" ? "HA-2022" : $"WB-{line.DoorCode}";
                var price = await pricing.CalculateAsync(new PricingRequest(door.Code, profile,
                    version.DefaultWidthMm, version.DefaultHeightMm, line.Quantity, date), ct);
                results.Add((door, line, price));
            }
        }
        catch (InvalidOperationException ex)
        {
            model.ErrorMessage = ex.Message;
            return View(model);
        }

        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var customerName = model.CustomerName.Trim();
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Name == customerName, ct);
        if (customer == null)
        {
            customer = new Customer { Code = $"C-{Guid.NewGuid():N}"[..20], Name = customerName };
            db.Customers.Add(customer);
        }
        Project? project = null;
        if (!string.IsNullOrWhiteSpace(model.ProjectName))
        {
            var projectName = model.ProjectName.Trim();
            project = customer.Id == 0 ? null : await db.Projects.FirstOrDefaultAsync(x => x.CustomerId == customer.Id && x.Name == projectName, ct);
            if (project == null)
            {
                project = new Project { Code = $"P-{Guid.NewGuid():N}"[..20], Name = projectName, Customer = customer };
                db.Projects.Add(project);
            }
        }
        var quote = new Quotation
        {
            Number = $"Q-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..29],
            Customer = customer,
            Project = project,
            CurrentRevisionNumber = 1
        };
        var revision = new QuotationRevision { Quotation = quote, RevisionNumber = 1, CalculationDateUtc = date };
        quote.Revisions.Add(revision);
        foreach (var (door, line, price) in results)
        {
            // The workbook rounds Redcon quoted unit prices to whole EGP; Hassan keeps cents.
            var quoted = door.Code.StartsWith("RED-", StringComparison.Ordinal)
                ? decimal.Round(price.UnitCalculatedPrice, 0, MidpointRounding.AwayFromZero)
                : price.UnitCalculatedPrice;
            var item = new QuotationItem
            {
                QuotationRevision = revision,
                DoorTemplate = door,
                WidthMm = price.WidthMm,
                HeightMm = price.HeightMm,
                Quantity = line.Quantity,
                CalculatedUnitPrice = price.UnitCalculatedPrice,
                OverrideUnitPrice = quoted == price.UnitCalculatedPrice ? null : quoted,
                OverrideReason = quoted == price.UnitCalculatedPrice ? null : "Workbook quotation rounding: whole EGP",
            };
            item.Snapshot = new CalculationSnapshot
            {
                QuotationItem = item,
                InputJson = JsonSerializer.Serialize(new { door.Code, line.Quantity, price.WidthMm, price.HeightMm, date }),
                BreakdownJson = JsonSerializer.Serialize(price.Components),
                ConfigurationJson = JsonSerializer.Serialize(new { price.PricingProfileCode, price.PricingProfileVersion, price.DoorTemplateVersion, price.Currency, quoted }),
                MaterialCost = price.MaterialCost,
                DryCost = price.DryCost,
                AdministrativeCost = price.AdministrativeCost,
                ProfitCost = price.ProfitCost,
                CalculatedPrice = price.UnitCalculatedPrice
            };
            revision.Items.Add(item);
        }
        db.Quotations.Add(quote);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return RedirectToAction(nameof(Details), new { id = quote.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var quote = await db.Quotations.AsNoTracking().Include(x => x.Customer).Include(x => x.Project)
            .Include(x => x.Revisions).ThenInclude(x => x.Items).ThenInclude(x => x.DoorTemplate)
            .SingleOrDefaultAsync(x => x.Id == id, ct);
        return quote == null ? NotFound() : View(quote);
    }

    private async Task Populate(QuotationDraftViewModel model, CancellationToken ct)
    {
        var versions = await db.DoorTemplates.AsNoTracking().Where(x => x.IsActive)
            .SelectMany(x => x.Versions.Select(v => new { x.Code, x.NameEn, x.NameAr, v.DefaultWidthMm, v.DefaultHeightMm, v.Version }))
            .ToListAsync(ct);
        model.AvailableDoors = versions.GroupBy(x => x.Code)
            .Select(g => g.OrderByDescending(x => x.Version).First())
            .Select(x => (x.Code, x.NameEn, x.NameAr, x.DefaultWidthMm, x.DefaultHeightMm))
            .ToList();
    }
}
