using DevExpress.Maui.DataForm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Models
{
    public class PersonalInfoModel
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
