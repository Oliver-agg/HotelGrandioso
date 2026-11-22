using Microsoft.EntityFrameworkCore;

// DbContext är EF Core:s koppling till databasen. Varje DbSet blir en tabell.
public class HotelContext : DbContext
{
    public DbSet<Rooms> Rooms => Set<Rooms>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<SpaBooking> SpaBookings => Set<SpaBooking>();

    // SQLite lagrar databasen i en vanlig fil (hotel.db), så ingen
    // databasserver behövs och projektet går att köra på alla datorer.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=hotel.db");
}