using System.ComponentModel.DataAnnotations;

namespace KarimDoors.Web.Models;

public sealed class AddSettingVersionViewModel
{
    public int SettingDefinitionId { get; set; }
    public string SettingName { get; set; } = string.Empty;

    [Required(ErrorMessage = "The {0} field is required.")]
    public string Value { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Effective from")]
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow.Date;

    [Required(ErrorMessage = "The {0} field is required."), StringLength(500)]
    [Display(Name = "Reason for change")]
    public string ChangeReason { get; set; } = string.Empty;
}
