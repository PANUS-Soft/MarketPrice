using DevExpress.Maui.DataForm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Models
{
    public class ContactAccountModel
    {
        // Contact and Account Info
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DataFormDisplayOptions(LabelText = "Email Address", GroupName = "Contact & Account Info")]
        [Display(Prompt = "Enter your email address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [DataFormDisplayOptions(LabelText = "Phone Number", GroupName = "Contact & Account Info")]
        [Display(Prompt = "Enter your phone number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select an account type")]
        [DataFormComboBoxEditor(IsFilterEnabled = false)]
        [DataFormDisplayOptions(LabelText = "Account Type", GroupName = "Contact & Account Info")]
        [Display(Prompt = "Select an account type")]
        public AccountType AccountType { get; set; }
    }
}
