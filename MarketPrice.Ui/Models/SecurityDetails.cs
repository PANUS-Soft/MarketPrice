using DevExpress.Maui.DataForm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Models
{
    public class SecurityDetails : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string password;
        private string confirmPassword;
        private readonly Dictionary<string, List<string>> _errors = new();

        // SecurityDetails Details
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataFormPasswordEditor]
        [DataFormDisplayOptions(LabelText = "Password", GroupName = "Security Details")]
        [DataType(DataType.Password)]
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
                ValidatePasswords();
            }
        }


        [Required(ErrorMessage = "Please confirm your password")]
        [Compare(nameof(Password), ErrorMessage = "Password do not match")]
        [DataFormPasswordEditor]
        [DataFormDisplayOptions(LabelText = "Confirm Password", GroupName = "Security Details")]
        [DataType(DataType.Password)]
        public string ConfirmPassword
        {
            get => confirmPassword; 
            set
            {
                confirmPassword = value;
                OnPropertyChanged();
                ValidatePasswords();
            }
        }
        [DataFormDisplayOptions(LabelText = "Accept Terms and Conditions")]
        [DataFormCheckBoxEditor]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms")]
        public bool IsTermAccepted { get; set; }

        private void ValidatePasswords()
        {
            ClearErrors(nameof(ConfirmPassword));
            if (!string.IsNullOrEmpty(ConfirmPassword) && Password != ConfirmPassword)
                AddError(nameof(ConfirmPassword), "Passwords do not match");
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();
            _errors[propertyName].Add(error);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        [DataFormDisplayOptions(IsVisible = false)]
        [Browsable(false)]
        public bool HasErrors => _errors.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public IEnumerable GetErrors(string propertyName) => _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
