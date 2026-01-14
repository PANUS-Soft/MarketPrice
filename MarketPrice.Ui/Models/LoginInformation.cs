using DevExpress.Maui.DataForm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Models
{
    public class LoginInformation
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DataFormDisplayOptions(LabelText = "Email Address")]
        [Display(Prompt = "Enter your email address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataFormPasswordEditor]
        [DataFormDisplayOptions(LabelText = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataFormDisplayOptions(LabelText = "Remeeber Me ?")]
        public bool RememberMe { get; set; }


    }
}
