using Microsoft.AspNetCore.SignalR;
using WarehouseDemoBackend.DTOs;
using WarehouseDemoBackend.Services;

namespace WarehouseDemoBackend.Hubs
{
    public interface ICoordsClient
    {
        Task ReceiveCoordinateUpdate(CoordinateDto coord);
    }

    public class CoordsHub : Hub<ICoordsClient>
    {
        private readonly ICoordinateService _coordService;

        public CoordsHub(ICoordinateService coordService)
        {
            _coordService = coordService;
        }

        public async Task RequestAllCoordinates()
        {
            var coords = await _coordService.GetAllCoordinatesAsync(CancellationToken.None);
            foreach (var coord in coords)
            {
                await Clients.Caller.ReceiveCoordinateUpdate(coord);
            }
        }
    }
}
