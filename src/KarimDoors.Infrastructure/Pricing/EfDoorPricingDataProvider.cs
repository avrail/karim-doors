using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Pricing;
using KarimDoors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Infrastructure.Pricing;

public sealed class EfDoorPricingDataProvider(KarimDoorsDbContext dbContext) : IDoorPricingDataProvider
{
    public async Task<PricingContext> ResolveAsync(
        PricingRequest request,
        CancellationToken cancellationToken = default)
    {
        var at = request.CalculationDateUtc;

        var template = await dbContext.DoorTemplates
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Code == request.DoorTemplateCode && x.IsActive, cancellationToken)
            ?? throw new InvalidOperationException($"Door template '{request.DoorTemplateCode}' was not found.");

        var templateVersion = await dbContext.DoorTemplateVersions
            .AsNoTracking()
            .Include(x => x.Components)
                .ThenInclude(x => x.Material)
                    .ThenInclude(x => x.Prices)
            .Where(x => x.DoorTemplateId == template.Id)
            .Where(x => x.EffectiveFromUtc <= at && (x.EffectiveToUtc == null || x.EffectiveToUtc > at))
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"No active version of '{request.DoorTemplateCode}' exists for {at:d}.");

        if (templateVersion.ReferenceSizeOnly &&
            (request.WidthMm != templateVersion.DefaultWidthMm || request.HeightMm != templateVersion.DefaultHeightMm))
        {
            throw new InvalidOperationException(
                $"{request.DoorTemplateCode} is validated only at {templateVersion.DefaultWidthMm} × {templateVersion.DefaultHeightMm} mm.");
        }

        if (request.PricingProfileCode.StartsWith("WB-", StringComparison.Ordinal) &&
            request.PricingProfileCode != $"WB-{template.Code}")
            throw new InvalidOperationException("The workbook pricing profile belongs to a different door reference case.");
        if (templateVersion.ReferenceSizeOnly && request.PricingProfileCode != $"WB-{template.Code}")
            throw new InvalidOperationException("This door reference case requires its matching workbook pricing profile.");

        var profile = await dbContext.PricingProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Code == request.PricingProfileCode && x.IsActive, cancellationToken)
            ?? throw new InvalidOperationException($"Pricing profile '{request.PricingProfileCode}' was not found.");

        var profileVersion = await dbContext.PricingProfileVersions
            .AsNoTracking()
            .Where(x => x.PricingProfileId == profile.Id)
            .Where(x => x.EffectiveFromUtc <= at && (x.EffectiveToUtc == null || x.EffectiveToUtc > at))
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"No active pricing profile version exists for {at:d}.");

        var components = templateVersion.Components
            .OrderBy(x => x.Sequence)
            .Select(rule =>
            {
                var price = rule.Material.Prices
                    .Where(x => x.EffectiveFromUtc <= at && (x.EffectiveToUtc == null || x.EffectiveToUtc > at))
                    .OrderByDescending(x => x.Version)
                    .FirstOrDefault()
                    ?? throw new InvalidOperationException(
                        $"No price exists for material '{rule.Material.Code}' on {at:d}.");

                return new PricingComponentInput(
                    rule.Code,
                    rule.NameEn,
                    rule.NameAr,
                    rule.Material.Code,
                    rule.Material.NameEn,
                    rule.Material.Unit,
                    price.UnitPrice,
                    price.Version,
                    rule.Formula,
                    rule.Quantity,
                    rule.MeasurementMultiplier,
                    rule.WastePercentage ?? profileVersion.DefaultWastePercentage,
                    rule.LengthSource,
                    rule.LengthOffsetMm,
                    rule.FixedLengthMm,
                    rule.WidthSource,
                    rule.WidthOffsetMm,
                    rule.FixedWidthMm,
                    rule.ThicknessMm,
                    rule.FixedMeasurement,
                    rule.Sequence);
            })
            .ToList();

        return new PricingContext(
            template.Code,
            template.NameEn,
            templateVersion.Version,
            templateVersion.FireRatingMinutes,
            request.WidthMm,
            request.HeightMm,
            new PricingProfileInput(
                profile.Code,
                profileVersion.Version,
                profileVersion.AdministrativePercentage,
                profileVersion.ProfitPercentage,
                profileVersion.ManufacturingCost,
                profileVersion.TransportCost,
                profileVersion.InstallationCost,
                profileVersion.Currency),
            components,
            at);
    }
}
