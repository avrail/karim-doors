using KarimDoors.Application.Abstractions;
using KarimDoors.Application.Settings;
using KarimDoors.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KarimDoors.Infrastructure.Persistence;

public sealed class SystemSettingsHistoryService(KarimDoorsDbContext dbContext) : ISystemSettingsHistoryService
{
    public async Task<IReadOnlyList<SettingSummary>> GetSettingsAsync(
        DateTime atUtc,
        CancellationToken cancellationToken = default)
    {
        var settings = await dbContext.SystemSettingDefinitions
            .AsNoTracking()
            .Include(x => x.Versions)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return settings.Select(setting =>
        {
            var current = setting.Versions
                .Where(x => x.EffectiveFromUtc <= atUtc && (x.EffectiveToUtc == null || x.EffectiveToUtc > atUtc))
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            return new SettingSummary(
                setting.Id,
                setting.Key,
                setting.Name,
                setting.Description,
                setting.ValueType,
                setting.IsPricingSensitive,
                current?.Value,
                current?.Version,
                current?.EffectiveFromUtc);
        }).ToList();
    }

    public async Task<SettingHistory?> GetHistoryAsync(
        int settingDefinitionId,
        CancellationToken cancellationToken = default)
    {
        var setting = await dbContext.SystemSettingDefinitions
            .AsNoTracking()
            .Include(x => x.Versions)
            .SingleOrDefaultAsync(x => x.Id == settingDefinitionId, cancellationToken);

        if (setting is null)
            return null;

        return new SettingHistory(
            setting.Id,
            setting.Key,
            setting.Name,
            setting.Description,
            setting.ValueType,
            setting.IsPricingSensitive,
            setting.Versions
                .OrderByDescending(x => x.Version)
                .Select(x => new SettingVersionHistoryItem(
                    x.Id,
                    x.Version,
                    x.Value,
                    x.EffectiveFromUtc,
                    x.EffectiveToUtc,
                    x.ChangeReason,
                    x.CreatedOnUtc,
                    x.CreatedBy))
                .ToList());
    }

    public async Task AddVersionAsync(
        AddSettingVersion command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.ChangeReason))
            throw new InvalidOperationException("A change reason is required for every setting change.");

        var setting = await dbContext.SystemSettingDefinitions
            .Include(x => x.Versions)
            .SingleOrDefaultAsync(x => x.Id == command.SettingDefinitionId, cancellationToken)
            ?? throw new InvalidOperationException("Setting was not found.");

        var latest = setting.Versions.OrderByDescending(x => x.Version).FirstOrDefault();
        if (latest is not null && command.EffectiveFromUtc <= latest.EffectiveFromUtc)
            throw new InvalidOperationException("The new effective date must be later than the latest version start date.");

        if (latest is not null && latest.EffectiveToUtc is null)
        {
            latest.EffectiveToUtc = command.EffectiveFromUtc;
            latest.ModifiedOnUtc = DateTime.UtcNow;
            latest.ModifiedBy = command.ChangedBy;
        }

        setting.Versions.Add(new SystemSettingVersion
        {
            Version = (latest?.Version ?? 0) + 1,
            EffectiveFromUtc = command.EffectiveFromUtc,
            Value = command.Value,
            ChangeReason = command.ChangeReason.Trim(),
            CreatedBy = command.ChangedBy
        });

        dbContext.AuditLogs.Add(new AuditLog
        {
            UserId = command.ChangedBy,
            EntityName = nameof(SystemSettingVersion),
            EntityKey = setting.Key,
            Action = "AddVersion",
            OldValuesJson = latest is null ? null : System.Text.Json.JsonSerializer.Serialize(new { latest.Version, latest.Value }),
            NewValuesJson = System.Text.Json.JsonSerializer.Serialize(new { Version = (latest?.Version ?? 0) + 1, command.Value }),
            Reason = command.ChangeReason
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
