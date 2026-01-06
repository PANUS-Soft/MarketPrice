using MarketPrice.Ui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Services.Session
{
    public class SessionStorage
    {
        private const string SessionKey = "UserSession";

        public async Task SaveAsync(UserSession session)
        {
            var json = JsonSerializer.Serialize(session);
            await SecureStorage.SetAsync(SessionKey, json);
        }

        public async Task<UserSession?> LoadAsync()
        {
            var json = await SecureStorage.GetAsync(SessionKey);
            return json == null ? null : JsonSerializer.Deserialize<UserSession>(json);
        }

        public void Clear()
        {
            SecureStorage.Remove(SessionKey);
        }
    }
}
