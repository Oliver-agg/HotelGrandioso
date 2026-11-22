public class Rooms
{
    public int Id { get; set; }
    public int RoomNumber { get; set; }
    public int Floor { get; set; }
    public int SizeM2 { get; set; }

    // Priset ligger på rummet och inte på RoomType, eftersom rum av typen
    // Standard har olika pris (800, 1 000 och 1 200 kr)
    public decimal PricePerNight { get; set; }

    // RoomTypeId är numret som sparas i databasen. RoomType är själva typen,
    // så att jag kan skriva room.RoomType.Name i koden.
    public int RoomTypeId { get; set; }
    public RoomType RoomType { get; set; } = null!;

    // Ett rum har många bokningar över tid
    public List<Booking> Bookings { get; set; } = new();
}