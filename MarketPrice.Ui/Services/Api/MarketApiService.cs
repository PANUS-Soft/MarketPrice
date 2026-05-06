using System.Net.Http.Headers;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.Services.Api
{
    public class MarketApiService
    {
        private readonly HttpClient _httpClient;
        private readonly SessionService _sessionService;

        // The absolute Base URL for the Android Emulator
        // (If you test on a Windows Machine instead of Android later, change this to "http://localhost:5278/")
        private readonly string _baseUrl = "http://10.0.2.2:5278/";

        public MarketApiService(HttpClient httpClient, SessionService sessionService)
        {
            _httpClient = httpClient;
            _sessionService = sessionService;
        }

        public async Task<HttpResponseMessage> GetMarketOverviewAsync(int positionTypeId)
        {
            var session = await _sessionService.GetCurrentSessionAsync();
            if (session != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
            }

            // Attached the base URL here!
            return await _httpClient.GetAsync($"{_baseUrl}Markets/overview/{positionTypeId}");
        }

        public async Task<HttpResponseMessage> GetCommodityMarketInsightAsync(Guid commodityId)
        {
            var session = await _sessionService.GetCurrentSessionAsync();
            if (session != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
            }

            // Attached the base URL here!
            return await _httpClient.GetAsync($"{_baseUrl}Markets/insight/{commodityId}");
        }

        public async Task<HttpResponseMessage> GetChartDataAsync(Guid commodityId, string range)
        {
            var session = await _sessionService.GetCurrentSessionAsync();
            if (session != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
            }

            // Attached the base URL here!
            return await _httpClient.GetAsync($"{_baseUrl}Markets/chart?commodityId={commodityId}&range={range}");
        }

        // I added this back in case your InsightViewModel or MarketViewModel still needs it!
        public async Task<HttpResponseMessage> LoadMarketAsync()
        {
            var session = await _sessionService.GetCurrentSessionAsync();
            if (session != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
            }

            // Attached the base URL here!
            return await _httpClient.GetAsync($"{_baseUrl}Markets");
        }
    }
}