using System.ComponentModel.DataAnnotations;

namespace KarimDoors.Web.Models;

public sealed class QuotationDraftViewModel
{
    [Required] public string CustomerName { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public List<QuotationDraftLine> Lines { get; set; } = [];
    public List<(string Code, string NameEn, string NameAr, int Width, int Height)> AvailableDoors { get; set; } = [];
    public string? ErrorMessage { get; set; }
}

public sealed class QuotationDraftLine
{
    public string? DoorCode { get; set; }
    public decimal Quantity { get; set; } = 1;
}
