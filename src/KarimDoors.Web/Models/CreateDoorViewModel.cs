using System.ComponentModel.DataAnnotations;
using KarimDoors.Domain.Entities;
using KarimDoors.Domain.Enums;

namespace KarimDoors.Web.Models;

public sealed class CreateDoorViewModel
{
    [Range(1, int.MaxValue)]
    [Display(Name = "Project")]
    public int ProjectId { get; set; }

    [Required, StringLength(30, MinimumLength = 2)]
    [RegularExpression("[A-Za-z0-9][A-Za-z0-9-]*")]
    [Display(Name = "Code")]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "English name")]
    public string NameEn { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "Arabic name")]
    public string NameAr { get; set; } = string.Empty;

    [Range(300, 5000)]
    [Display(Name = "Width (mm)")]
    public int WidthMm { get; set; }

    [Range(300, 5000)]
    [Display(Name = "Height (mm)")]
    public int HeightMm { get; set; } = 2200;

    [Range(0, 240)]
    [Display(Name = "Fire rating (minutes)")]
    public int FireRatingMinutes { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    [Display(Name = "Manufacturing")]
    public decimal ManufacturingCost { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    [Display(Name = "Transport")]
    public decimal TransportCost { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    [Display(Name = "Installation")]
    public decimal InstallationCost { get; set; }

    [Range(typeof(decimal), "0", "100")]
    [Display(Name = "Administrative percentage")]
    public decimal AdministrativePercentage { get; set; } = 17.5m;

    [Range(typeof(decimal), "0", "100")]
    [Display(Name = "Profit percentage")]
    public decimal ProfitPercentage { get; set; } = 25m;

    [Range(0, 2)]
    [Display(Name = "Quotation rounding")]
    public int QuoteRoundingDigits { get; set; } = 2;

    public List<CreateDoorComponentViewModel> Components { get; set; } = [new()];

    public IReadOnlyList<Project> Projects { get; set; } = [];
}

public sealed class CreateDoorComponentViewModel
{
    public string? Code { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public MaterialCategory Category { get; set; } = MaterialCategory.Other;
    public MaterialUnit Unit { get; set; } = MaterialUnit.Piece;
    public decimal Measurement { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal WastePercentage { get; set; }
}
