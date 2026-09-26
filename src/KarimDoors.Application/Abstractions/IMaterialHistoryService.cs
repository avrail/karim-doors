using KarimDoors.Application.Materials;

namespace KarimDoors.Application.Abstractions;

public interface IMaterialHistoryService
{
    Task<IReadOnlyList<MaterialSummary>> GetMaterialsAsync(
        DateTime atUtc,
        CancellationToken cancellationToken = default);

    Task<MaterialHistory?> GetHistoryAsync(
        int materialId,
        CancellationToken cancellationToken = default);

    Task AddPriceVersionAsync(
        AddMaterialPriceVersion command,
        CancellationToken cancellationToken = default);
}
