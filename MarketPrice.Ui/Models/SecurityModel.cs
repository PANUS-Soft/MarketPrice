using DevExpress.Maui.DataForm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Models
{
    public class SecurityModel
    {
        // Security Details
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataFormPasswordEditor]
        [DataFormDisplayOptions(LabelText = "Password", GroupName = "Security Details")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare(nameof(Password), ErrorMessage = "Password do not match")]
        [DataFormPasswordEditor]
        [DataFormDisplayOptions(LabelText = "Confirm Password", GroupName = "Security Details")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms")]
        [DataFormCheckBoxEditor]
        [DataFormDisplayOptions(LabelText = "By creating an account, you agree to MarketPrice’s Terms of Services and Privacy Policy", GroupName = "Security Details")]
        public bool IsTermAccepted { get; set; }
    }
}
