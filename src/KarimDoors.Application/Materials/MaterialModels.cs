using KarimDoors.Domain.Enums;

namespace KarimDoors.Application.Materials;

public sealed record MaterialSummary(
    int Id,
    string Code,
    string NameEn,
    string NameAr,
    MaterialCategory Category,
    MaterialUnit Unit,
    decimal? CurrentPrice,
    string? Currency,
    int? CurrentVersion,
    DateTime? EffectiveFromUtc);

public sealed record MaterialPriceHistoryItem(
    int Id,
    int Version,
    decimal UnitPrice,
    string Currency,
    DateTime EffectiveFromUtc,
    DateTime? EffectiveToUtc,
    string? ChangeReason,
    string? SupplierReference,
    DateTime CreatedOnUtc,
    string? CreatedBy);

public sealed record MaterialHistory(
    int Id,
    string Code,
    string NameEn,
    string NameAr,
    MaterialCategory Category,
    MaterialUnit Unit,
    IReadOnlyList<MaterialPriceHistoryItem> Prices);

public sealed record AddMaterialPriceVersion(
    int MaterialId,
    decimal UnitPrice,
    string Currency,
    DateTime EffectiveFromUtc,
    string ChangeReason,
    string? SupplierReference,
    string? ChangedBy);
