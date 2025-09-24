namespace WarehouseDemoBackend.Models
{
    public class Coordinate
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public double Lat { get; set; }
        public double Lon { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
