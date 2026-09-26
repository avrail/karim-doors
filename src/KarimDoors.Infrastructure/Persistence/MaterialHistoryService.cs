using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Materials;
using KarimDoors.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Infrastructure.Persistence;

public sealed class MaterialHistoryService(KarimDoorsDbContext dbContext) : IMaterialHistoryService
{
    public async Task<IReadOnlyList<MaterialSummary>> GetMaterialsAsync(
        DateTime atUtc,
        CancellationToken cancellationToken = default)
    {
        var materials = await dbContext.Materials
            .AsNoTracking()
            .Include(x => x.Prices)
            .OrderBy(x => x.Category)
            .ThenBy(x => x.NameEn)
            .ToListAsync(cancellationToken);

        return materials.Select(material =>
        {
            var current = material.Prices
                .Where(x => x.EffectiveFromUtc <= atUtc && (x.EffectiveToUtc == null || x.EffectiveToUtc > atUtc))
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            return new MaterialSummary(
                material.Id,
                material.Code,
                material.NameEn,
                material.NameAr,
                material.Category,
                material.Unit,
                current?.UnitPrice,
                current?.Currency,
                current?.Version,
                current?.EffectiveFromUtc);
        }).ToList();
    }

    public async Task<MaterialHistory?> GetHistoryAsync(
        int materialId,
        CancellationToken cancellationToken = default)
    {
        var material = await dbContext.Materials
            .AsNoTracking()
            .Include(x => x.Prices)
            .SingleOrDefaultAsync(x => x.Id == materialId, cancellationToken);

        if (material is null)
            return null;

        return new MaterialHistory(
            material.Id,
            material.Code,
            material.NameEn,
            material.NameAr,
            material.Category,
            material.Unit,
            material.Prices
                .OrderByDescending(x => x.Version)
                .Select(x => new MaterialPriceHistoryItem(
                    x.Id,
                    x.Version,
                    x.UnitPrice,
                    x.Currency,
                    x.EffectiveFromUtc,
                    x.EffectiveToUtc,
                    x.ChangeReason,
                    x.SupplierReference,
                    x.CreatedOnUtc,
                    x.CreatedBy))
                .ToList());
    }

    public async Task AddPriceVersionAsync(
        AddMaterialPriceVersion command,
        CancellationToken cancellationToken = default)
    {
        if (command.UnitPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(command.UnitPrice));
        if (string.IsNullOrWhiteSpace(command.ChangeReason))
            throw new InvalidOperationException("A change reason is required for every price change.");

        var material = await dbContext.Materials
            .Include(x => x.Prices)
            .SingleOrDefaultAsync(x => x.Id == command.MaterialId, cancellationToken)
            ?? throw new InvalidOperationException("Material was not found.");

        var latest = material.Prices.OrderByDescending(x => x.Version).FirstOrDefault();
        if (latest is not null && command.EffectiveFromUtc <= latest.EffectiveFromUtc)
            throw new InvalidOperationException("The new effective date must be later than the latest version start date.");

        if (latest is not null && latest.EffectiveToUtc is null)
        {
            latest.EffectiveToUtc = command.EffectiveFromUtc;
            latest.ModifiedOnUtc = DateTime.UtcNow;
            latest.ModifiedBy = command.ChangedBy;
        }

        material.Prices.Add(new MaterialPrice
        {
            Version = (latest?.Version ?? 0) + 1,
            EffectiveFromUtc = command.EffectiveFromUtc,
            UnitPrice = command.UnitPrice,
            Currency = command.Currency.Trim().ToUpperInvariant(),
            ChangeReason = command.ChangeReason.Trim(),
            SupplierReference = string.IsNullOrWhiteSpace(command.SupplierReference)
                ? null
                : command.SupplierReference.Trim(),
            CreatedBy = command.ChangedBy
        });

        dbContext.AuditLogs.Add(new AuditLog
        {
            UserId = command.ChangedBy,
            EntityName = nameof(MaterialPrice),
            EntityKey = material.Code,
            Action = "AddVersion",
            OldValuesJson = latest is null
                ? null
                : System.Text.Json.JsonSerializer.Serialize(new
                {
                    latest.Version,
                    latest.UnitPrice,
                    latest.Currency,
                    latest.EffectiveFromUtc
                }),
            NewValuesJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                Version = (latest?.Version ?? 0) + 1,
                command.UnitPrice,
                command.Currency,
                command.EffectiveFromUtc
            }),
            Reason = command.ChangeReason
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
