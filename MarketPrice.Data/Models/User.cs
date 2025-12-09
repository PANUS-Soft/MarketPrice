using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public required int AccountTypeId { get; set; }
        public required string FirstName { get; set; }
        public required string FamilyName { get; set; }
        public required string OtherNames { get; set; }
        public required string IdCardNumber { get; set; }
        public required string EmailAddress { get; set; }
        public required string  PhoneNumber { get; set; }
        public required bool IsPremiumUser { get; set; }
        public required string PasswordHash { get; set; }
        public required DateTime DateRecorded { get; set; }
        public string? Note { get; set; }


    }
}
