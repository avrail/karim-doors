using KarimDoors.Application.Lookups;
using KarimDoors.Application.Pricing;
using System.ComponentModel.DataAnnotations;

namespace KarimDoors.Web.Models;

public sealed class PricingCalculatorViewModel
{
    [Required(ErrorMessage = "The {0} field is required.")]
    [Display(Name = "Door template")]
    public string DoorTemplateCode { get; set; } = "HA-D04";


    [Range(300, 5000, ErrorMessage = "The field {0} must be between {1} and {2}.")]
    [Display(Name = "Width (mm)")]
    public int WidthMm { get; set; } = 970;

    [Range(300, 5000, ErrorMessage = "The field {0} must be between {1} and {2}.")]
    [Display(Name = "Height (mm)")]
    public int HeightMm { get; set; } = 2200;

    [Range(typeof(decimal), "0.01", "100000", ErrorMessage = "The field {0} must be between {1} and {2}.")]
    [Display(Name = "Quantity")]
    public decimal Quantity { get; set; } = 1m;

    [DataType(DataType.Date)]
    [Display(Name = "Pricing date")]
    public DateTime CalculationDate { get; set; } = DateTime.UtcNow.Date;

    [Range(1, int.MaxValue)]
    [Display(Name = "Project")]
    public int ProjectId { get; set; }
    public IReadOnlyList<PricingProjectOption> Projects { get; set; } = [];
    public IReadOnlyList<DoorTemplateOption> DoorTemplates { get; set; } = [];
    public PricingResult? Result { get; set; }
    public string? ErrorMessage { get; set; }
}
