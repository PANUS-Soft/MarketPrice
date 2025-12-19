using System.ComponentModel.DataAnnotations;
using DevExpress.Maui.DataForm;

namespace MarketPrice.Ui.Models
{
    public class UserRegistrationModel : BindableObject
    {
        public PersonalInfoModel PersonalInfo { get; set; }
        public ContactAccountModel ContactAccountInfo { get; set; }
        public SecurityModel SecurityInfo { get; set; }
    }

    public enum AccountType {Personal, Business}
}