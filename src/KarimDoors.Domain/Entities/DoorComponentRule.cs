using KarimDoors.Domain.Common;
using KarimDoors.Domain.Enums;

namespace KarimDoors.Domain.Entities;

public sealed class DoorComponentRule : AuditableEntity
{
    public int DoorTemplateVersionId { get; set; }
    public DoorTemplateVersion DoorTemplateVersion { get; set; } = null!;
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public int Sequence { get; set; }

    public MeasurementFormula Formula { get; set; }
    public decimal Quantity { get; set; } = 1m;
    public decimal MeasurementMultiplier { get; set; } = 1m;
    public decimal? WastePercentage { get; set; }

    public DimensionSource LengthSource { get; set; }
    public decimal LengthOffsetMm { get; set; }
    public decimal? FixedLengthMm { get; set; }

    public DimensionSource WidthSource { get; set; }
    public decimal WidthOffsetMm { get; set; }
    public decimal? FixedWidthMm { get; set; }

    public decimal? ThicknessMm { get; set; }
    public decimal? FixedMeasurement { get; set; }
}
