using KarimDoors.Application.Settings;

namespace KarimDoors.Application.Abstractions;

public interface ISystemSettingsHistoryService
{
    Task<IReadOnlyList<SettingSummary>> GetSettingsAsync(
        DateTime atUtc,
        CancellationToken cancellationToken = default);

    Task<SettingHistory?> GetHistoryAsync(
        int settingDefinitionId,
        CancellationToken cancellationToken = default);

    Task AddVersionAsync(
        AddSettingVersion command,
        CancellationToken cancellationToken = default);
}
