using System.ComponentModel.DataAnnotations;

namespace KarimDoors.Web.Models;

public sealed class AddMaterialPriceViewModel
{
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.0001", "999999999")]
    [Display(Name = "Unit price")]
    public decimal UnitPrice { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "EGP";

    [DataType(DataType.Date)]
    [Display(Name = "Effective from")]
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow.Date;

    [Required, StringLength(500)]
    [Display(Name = "Reason for change")]
    public string ChangeReason { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Supplier reference")]
    public string? SupplierReference { get; set; }
}
