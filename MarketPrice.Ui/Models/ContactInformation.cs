using DevExpress.Maui.DataForm;
using System.ComponentModel.DataAnnotations;

namespace MarketPrice.Ui.Models
{
    public class ContactInformation
    {
        // Contact and Account Info
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DataFormDisplayOptions(LabelText = "Email Address", GroupName = "Contact & Account Info")]
        [Display(Prompt = "Enter your email address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [MaxLength(9, ErrorMessage = "Phone number can only be 9 digits")]
        [MinLength(9, ErrorMessage = "Phone number can't be less than 9 digits")]
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
    public enum AccountType
    {
        Personal,
        Business

    }
}
