using Microsoft.AspNetCore.SignalR;
using WarehouseDemoBackend.Data;
using WarehouseDemoBackend.DTOs;
using WarehouseDemoBackend.Hubs;
using WarehouseDemoBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace WarehouseDemoBackend.Services
{
    public interface ICoordinateService
    {
        Task<CoordinateDto> UpdateCoordinateAsync(int id, double lat, double lon, CancellationToken ct);
        Task<List<CoordinateDto>> GetAllCoordinatesAsync(CancellationToken ct);
    }

    public class CoordinateService : ICoordinateService
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<CoordsHub, ICoordsClient> _hub;

        public CoordinateService(AppDbContext db, IHubContext<CoordsHub, ICoordsClient> hub)
        {
            _db = db;
            _hub = hub;
        }

        public async Task<CoordinateDto>  UpdateCoordinateAsync(int id, double lat, double lon, CancellationToken ct)
        {
            var coord = await _db.Coordinates.FindAsync(new object[] { id }, ct);
            if (coord == null)
            {
                coord = new Coordinate
                {
                    Id = id,
                    Name = $"Coord{id}",
                    Lat = lat,
                    Lon = lon,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.Coordinates.Add(coord);
            }
            else
            {
                coord.Lat = lat;
                coord.Lon = lon;
                coord.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(ct);

            var dto = new CoordinateDto
            {
                Id = coord.Id,
                Name = coord.Name,
                Lat = coord.Lat,
                Lon = coord.Lon,
                UpdatedAt = coord.UpdatedAt
            };

            await _hub.Clients.All.ReceiveCoordinateUpdate(dto);

            return dto;
        }

        public async Task<List<CoordinateDto>> GetAllCoordinatesAsync(CancellationToken ct)
        {
            return await _db.Coordinates
                .Select(c => new CoordinateDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Lat = c.Lat,
                    Lon = c.Lon,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync(ct);
        }
    }
}
