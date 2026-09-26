namespace KarimDoors.Web.Models;

public sealed class DashboardViewModel
{
    public int MaterialsCount { get; init; }
    public int DoorTemplatesCount { get; init; }
    public int PricingProfilesCount { get; init; }
    public int QuotationsCount { get; init; }
    public int AuditEventsCount { get; init; }
    public DateTime GeneratedOnUtc { get; init; }
}
