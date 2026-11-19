using Microsoft.EntityFrameworkCore;

public class HotelContext : DbContext
{
    public DbSet<Rooms> Rooms => Set<Rooms>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=hotel.db");
}