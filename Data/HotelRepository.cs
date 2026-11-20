using Microsoft.EntityFrameworkCore;
public class HotelRepository
{
    private readonly HotelContext _db;

    public HotelRepository(HotelContext db)
    {
        _db = db;
    }

    public List<Rooms> GetAllRooms()
    {
        return _db.Rooms
            .Include(r => r.RoomType)
            .OrderBy(r => r.RoomNumber)
            .ToList();
    }

    public Rooms? GetRoomByNumber(int roomNumber)
    {
        return _db.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefault(r => r.RoomNumber == roomNumber);
    }

    public Booking AddBooking(string guestName, Rooms room, DateOnly checkIn, DateOnly checkOut)
    {
        Guest? guest = _db.Guests.FirstOrDefault(g => g.Name == guestName);
        if (guest == null)
        {
            guest = new Guest { Name = guestName };
        }

        var booking = new Booking
        {
            Guest = guest,
            Room = room,
            CheckIn = checkIn,
            CheckOut = checkOut
        };

        _db.Bookings.Add(booking);
        _db.SaveChanges();
        return booking;
    }

    public decimal CalculateTotalPrice(Rooms room, DateOnly checkIn, DateOnly checkOut)
    {
        int nights = checkOut.DayNumber - checkIn.DayNumber;
        return nights * room.PricePerNight;
    }

    public List<Booking> GetAllBookings()
    {
        return _db.Bookings
            .Include(b => b.Guest)
            .Include(b => b.Room)
            .OrderBy(b => b.CheckIn)
            .ToList();
    }

    public bool CancelBooking(int bookingId)
    {
        Booking? booking = _db.Bookings.Find(bookingId);

        if (booking == null)
        {
            return false;
        }

        _db.Bookings.Remove(booking);
        _db.SaveChanges();
        return true;
    }
}