using Microsoft.EntityFrameworkCore;

// Repositoryt sköter allt som har med databasen att göra. Menyn ber den om
// saker, men pratar aldrig själv med databasen.
public class HotelRepository
{
    private readonly HotelContext _db;

    // Databasen skickas in utifrån i stället för att skapas här, så att
    // Program.cs bestämmer när den öppnas och stängs
    public HotelRepository(HotelContext db)
    {
        _db = db;
    }

    public List<Rooms> GetAllRooms()
    {
        // Include hämtar rumstypen tillsammans med rummet. Utan Include är
        // room.RoomType null och då går det inte att skriva ut typens namn.
        // Jag sorterar på RoomNumber och inte på pris, eftersom SQLite sparar
        // decimal som text och inte kan sortera på det.
        return _db.Rooms
            .Include(r => r.RoomType)
            .OrderBy(r => r.RoomNumber)
            .ToList();
    }

    public Rooms? GetRoomByNumber(int roomNumber)
    {
        // FirstOrDefault ger null om rummet inte finns i stället för att
        // krascha. Menyn kollar null och skriver ett felmeddelande.
        return _db.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefault(r => r.RoomNumber == roomNumber);
    }

    public List<Rooms> GetAvailableRooms(DateOnly from, DateOnly to)
    {
        // Ett rum är ledigt om det inte finns någon bokning på det som
        // överlappar perioden. Det är samma regel som i IsRoomAvailable, men
        // här hämtar jag alla lediga rum i stället för att svara ja eller nej
        // på ett rum. Any blir EXISTS i SQL.
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
        // Kontrollen ligger här där bokningen sparas och inte bara i menyn,
        // så att den inte går att gå runt. Den kommer före gästen skapas, så
        // att ingen gäst sparas när bokningen nekas. null betyder nekad.
        if (!IsRoomAvailable(room.Id, checkIn, checkOut))
        {
            return null;
        }

        Guest guest = FindOrCreateGuest(guestName, email, phoneNumber);

        // Jag sätter hela objekten (Guest och Room) och inte id-talen. Då
        // fyller EF i GuestId och RoomId själv och sparar en ny gäst också.
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
        // Priset räknas ut i C# och inte i databasen, eftersom SQLite sparar
        // decimal som text och inte kan räkna på det
        int nights = checkOut.DayNumber - checkIn.DayNumber;
        return nights * room.PricePerNight;
    }

    public List<Booking> GetAllBookings()
    {
        // Include hämtar gäst och rum tillsammans med bokningen. Utan Include
        // är booking.Guest och booking.Room null när jag skriver ut.
        return _db.Bookings
            .Include(b => b.Guest)
            .Include(b => b.Room)
            .OrderBy(b => b.CheckIn)
            .ToList();
    }

    public bool CancelBooking(int bookingId)
    {
        // Find söker på id och ger null om bokningen inte finns. Jag svarar
        // true eller false så att menyn kan välja rätt meddelande utan att
        // veta något om databasen.
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
        // Rummet är upptaget om det finns en bokning på just det rummet som
        // överlappar. Jag använder < och inte <=, så att en gäst kan checka in
        // samma dag som en annan checkar ut. Any blir EXISTS i SQL, så det är
        // databasen som gör jobbet och inte C#.
        bool hasOverlap = _db.Bookings.Any(b =>
            b.RoomId == roomId &&
            b.CheckIn < checkOut &&
            checkIn < b.CheckOut);

        return !hasOverlap;
    }

    public List<Treatment> GetAllTreatments()
    {
        // Sorterat på id och inte på pris, eftersom SQLite inte kan sortera
        // decimal i databasen
        return _db.Treatments
            .OrderBy(t => t.Id)
            .ToList();
    }

    public Treatment? GetTreatmentById(int treatmentId)
    {
        // Find söker på id och ger null om behandlingen inte finns
        return _db.Treatments.Find(treatmentId);
    }

    public SpaBooking AddSpaBooking(string guestName, Treatment treatment, DateOnly date)
    {
        // Gästen hämtas eller skapas på samma sätt som vid rumsbokning
        Guest guest = FindOrCreateGuest(guestName, null, null);

        // Hela objekten sätts, så fyller EF i GuestId och TreatmentId själv
        var spaBooking = new SpaBooking
        {
            Guest = guest,
            Treatment = treatment,
            Date = date
        };

        _db.SpaBookings.Add(spaBooking);
        _db.SaveChanges();
        return spaBooking;
    }

    private Guest FindOrCreateGuest(string guestName, string? email, string? phoneNumber)
    {
        // Samma namn använder samma gäst. Det är en avgränsning som står i
        // README: två personer med samma namn slås ihop. En ny gäst sparas
        // inte här, den följer med när bokningen sparas.
        Guest? guest = _db.Guests.FirstOrDefault(g => g.Name == guestName);
        if (guest == null)
        {
            guest = new Guest { Name = guestName, Email = email, PhoneNumber = phoneNumber };
        }

        return guest;
    }
}