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
    public List<Rooms> GetAvailableRooms(DateOnly from, DateOnly to)
    {
        return _db.Rooms
            .Where(r => !_db.Bookings.Any(b =>
                b.RoomId == r.Id &&
                b.CheckIn < to &&
                b.CheckOut > from))
            .Include(r => r.RoomType)
            .OrderBy(r => r.RoomNumber)
            .ToList();
    }

    public Booking? AddBooking(string guestName, Rooms room, DateOnly checkIn, DateOnly checkOut, string? email = null, string? phoneNumber = null)
    {
        if (!IsRoomAvailable(room.Id, checkIn, checkOut))
        {
            return null;
        }

        Guest? guest = _db.Guests.FirstOrDefault(g => g.Name == guestName);
        if (guest == null)
        {
            guest = new Guest { Name = guestName, Email = email, PhoneNumber = phoneNumber };
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
    public bool IsRoomAvailable(int roomId, DateOnly checkIn, DateOnly checkOut)
    {
        bool hasOverlap = _db.Bookings.Any(b =>
            b.RoomId == roomId &&
            b.CheckIn < checkOut &&
            checkIn < b.CheckOut);

        return !hasOverlap;
    }
}
