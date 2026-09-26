using KarimDoors.Domain.Enums;

namespace KarimDoors.Application.Settings;

public sealed record SettingSummary(
    int Id,
    string Key,
    string Name,
    string? Description,
    SettingValueType ValueType,
    bool IsPricingSensitive,
    string? CurrentValue,
    int? CurrentVersion,
    DateTime? EffectiveFromUtc);

public sealed record SettingVersionHistoryItem(
    int Id,
    int Version,
    string Value,
    DateTime EffectiveFromUtc,
    DateTime? EffectiveToUtc,
    string? ChangeReason,
    DateTime CreatedOnUtc,
    string? CreatedBy);

public sealed record SettingHistory(
    int Id,
    string Key,
    string Name,
    string? Description,
    SettingValueType ValueType,
    bool IsPricingSensitive,
    IReadOnlyList<SettingVersionHistoryItem> Versions);

public sealed record AddSettingVersion(
    int SettingDefinitionId,
    string Value,
    DateTime EffectiveFromUtc,
    string ChangeReason,
    string? ChangedBy);
