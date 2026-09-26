using KarimDoors.Application.Lookups;
using KarimDoors.Application.Pricing;
using System.ComponentModel.DataAnnotations;

namespace KarimDoors.Web.Models;

public sealed class PricingCalculatorViewModel
{
    [Required(ErrorMessage = "The {0} field is required.")]
    [Display(Name = "Door template")]
    public string DoorTemplateCode { get; set; } = "HA-D04";

    [Required(ErrorMessage = "The {0} field is required.")]
    [Display(Name = "Pricing profile")]
    public string PricingProfileCode { get; set; } = "HA-2022";

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

    public IReadOnlyList<LookupOption> DoorTemplates { get; set; } = [];
    public IReadOnlyList<LookupOption> PricingProfiles { get; set; } = [];
    public PricingResult? Result { get; set; }
    public string? ErrorMessage { get; set; }
}
