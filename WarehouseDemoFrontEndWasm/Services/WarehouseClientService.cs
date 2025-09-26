using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;


namespace WarehouseDemoFrontEndWasm.Services
{

    public class WarehouseClientService
    {
        private readonly HttpClient _http;
        private HubConnection? _hub;

        public event Action<WarehouseDto>? OnWarehouseUpdated;

        public WarehouseClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task StartHubAsync()
        {
            _hub = new HubConnectionBuilder()
                .WithUrl("https://localhost:7209/warehouseHub") // backend hub endpoint
                .WithAutomaticReconnect()
                .Build();

            _hub.On<WarehouseDto>("WarehouseUpdated", warehouse =>
            {
                OnWarehouseUpdated?.Invoke(warehouse);
            });

            await _hub.StartAsync();
        }

        public async Task<WarehouseDto?> GetWarehouseAsync()
        {
            return await _http.GetFromJsonAsync<WarehouseDto>("warehouse/data");
        }

        public async Task AddBotAsync(AddBotRequest request)
        {
            await _http.PostAsJsonAsync("warehouse/bot/add", request);
        }

        public async Task RemoveBotAsync(int id)
        {
            await _http.PostAsJsonAsync($"warehouse/bot/remove/{id}", new { });
        }
    }

    // DTOs to match backend responses
    public record WarehouseDto(List<BotDto> ActiveBots);
    public record BotDto(int Id, string Color, float X, float Y);
    public record AddBotRequest(float X, float Y, string DefaultColor, double StartingStepSpeed);
}