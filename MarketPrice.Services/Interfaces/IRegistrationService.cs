using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IRegistrationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>

        public bool UserExists(string UserId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="FamilyName"></param>
        /// <param name="OtherName"></param>
        /// <param name="AccountName"></param>
        /// <param name="EmailAddress"></param>
        /// <param name="PhoneNumber"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public string Register(string FirstName, string FamilyName, string OtherName, string AccountName, string EmailAddress, string PhoneNumber, string Password);

    }
}
