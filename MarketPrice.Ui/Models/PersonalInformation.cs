using DevExpress.Maui.DataForm;
using System.ComponentModel.DataAnnotations;

namespace MarketPrice.Ui.Models
{
    public class PersonalInformation
    {
        // Personal Information
        [Required(ErrorMessage = "First Name is required")]
        [DataFormDisplayOptions(LabelText = "First Name", GroupName = "Personal Information")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Family name is required")]
        [DataFormDisplayOptions(LabelText = "Family Name", GroupName = "Personal Information")]
        public string FamilyName { get; set; }

        [DataFormDisplayOptions(LabelText = "Other Name", GroupName = "Personal Information")]
        public string? OtherName { get; set; }

    }
}
