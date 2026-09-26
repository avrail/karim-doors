namespace KarimDoors.Application.Lookups;

public sealed record DoorTemplateOption(
    string Code,
    string NameEn,
    string NameAr,
    int ProjectId,
    int WidthMm,
    int HeightMm,
    bool ReferenceSizeOnly);

public sealed record PricingProjectOption(int Id, string Code, string Name);
