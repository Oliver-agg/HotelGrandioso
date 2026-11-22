using Microsoft.EntityFrameworkCore;

// HotelContext är kopplingen mellan koden och databasen.
// Varje DbSet blir en tabell.
public class HotelContext : DbContext
{
    public DbSet<Rooms> Rooms => Set<Rooms>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<SpaBooking> SpaBookings => Set<SpaBooking>();

    // SQLite sparar databasen i en vanlig fil (hotel.db). Då behövs ingen
    // databasserver och projektet går att köra på vilken dator som helst.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=hotel.db");
}