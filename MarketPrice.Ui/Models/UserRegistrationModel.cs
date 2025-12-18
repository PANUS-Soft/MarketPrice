using System.ComponentModel.DataAnnotations;
using DevExpress.Maui.DataForm;

namespace MarketPrice.Ui.Models
{
    public class UserRegistrationModel : BindableObject
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

        // Contact and Account Info
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DataFormDisplayOptions(LabelText = "Email Address", GroupName = "Contact & Account Info")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [DataFormDisplayOptions(LabelText = "Phone Number", GroupName = "Contact & Account Info")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select an account type")]
        [DataFormComboBoxEditor(IsFilterEnabled = false)]
        [DataFormDisplayOptions(LabelText = "Account Type", GroupName = "Contact & Account Info")]
        public AccountType AccountType { get; set; }

        // Security Details
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataFormPasswordEditor]
        [DataFormDisplayOptions(LabelText = "Password", GroupName = "Security Details")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare(nameof(Password), ErrorMessage = "Password do not match")]
        [DataFormPasswordEditor]
        [DataFormDisplayOptions(LabelText = "Confirm Password", GroupName = "Security Details")]
        public string ConfirmPassword { get; set; }

        private bool isTermsAccepted;
        [DataFormCheckBoxEditor]
        [Required(ErrorMessage = "You must accept the terms")]
        [DataFormDisplayOptions(LabelText = "By creating an account, you agree to MarketPrice’s Terms of Services and Privacy Policy", GroupName = "Security Details")]
        public bool IsTermAccepted
        {
            get => isTermsAccepted;
            set { isTermsAccepted = value; OnPropertyChanged(); }
        }
    }

    public enum AccountType {Personal, Business}
}