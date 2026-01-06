using MarketPrice.Ui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Services.Session
{
    public class SessionService
    {
        public UserSession CurrentSession { get; private set; }

        public bool IsAuthenticated => CurrentSession != null && CurrentSession.ExpireAt > DateTime.UtcNow;

        public void StartSession(UserSession session)
        {
            CurrentSession = session;
        }

        public void EndSession()
        {
            CurrentSession = null;
        }   
    }
}
