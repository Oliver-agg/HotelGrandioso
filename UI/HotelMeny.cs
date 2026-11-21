using System.Globalization;

public class HotelMenu
{
    private readonly HotelRepository _repo;

    public HotelMenu(HotelRepository repo)
    {
        _repo = repo;
    }

    public void Run()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("=== Hotel Grandioso ===");
            Console.WriteLine();
            Console.WriteLine("1. Boka rum");
            Console.WriteLine("2. Visa alla rum");
            Console.WriteLine("3. Visa bokningar");
            Console.WriteLine("4. Avboka");
            Console.WriteLine("5. Sök efter lediga rum");
            Console.WriteLine("0. Avsluta");
            Console.WriteLine();
            Console.Write("Val: ");
            string? choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    BookRoom();
                    break;
                case "2":
                    ShowAllRooms();
                    break;
                case "3":
                    ShowAllBookings();
                    break;
                case "4":
                    CancelBooking();
                    break;
                case "5":
                    SearchAvailableRooms();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Ogiltigt val.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private void ShowAllRooms()
    {
        Console.WriteLine("Alla rum:");

        foreach (Rooms room in _repo.GetAllRooms())
        {
            Console.WriteLine($"{room.RoomNumber} - {room.RoomType.Name}  {room.SizeM2} m²  {room.PricePerNight:0} kr/natt");
        }
    }

    private void BookRoom()
    {
        Console.WriteLine("--- Boka rum ---");

        Console.Write("Gästnamn: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Namnet får inte vara tomt.");
            return;
        }

        Console.Write("E-post (valfritt, tryck Enter för att hoppa över): ");
        string? email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email)) email = null;

        Console.Write("Telefonnummer (valfritt, tryck Enter för att hoppa över): ");
        string? phone = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(phone)) phone = null;

        DateOnly checkIn = ReadDate("Incheckning (YYYY-MM-DD): ");
        DateOnly checkOut = ReadDate("Utcheckning (YYYY-MM-DD): ");

        if (checkOut <= checkIn)
        {
            Console.WriteLine("Utcheckningen måste vara efter incheckningen.");
            return;
        }

        Console.WriteLine();
        ShowAllRooms();
        Console.WriteLine();

        Console.Write("Rumsnummer: ");
        string? roomText = Console.ReadLine();
        if (!int.TryParse(roomText, out int roomNumber))
        {
            Console.WriteLine("Rumsnumret måste vara ett tal.");
            return;
        }

        Rooms? room = _repo.GetRoomByNumber(roomNumber);
        if (room == null)
        {
            Console.WriteLine($"Rum {roomNumber} finns inte.");
            return;
        }

        Booking? booking = _repo.AddBooking(name, room, checkIn, checkOut, email, phone);
        if (booking == null)
        {
            Console.WriteLine($"Rum {roomNumber} är redan bokat de datumen.");
            return;
        }

        int nights = checkOut.DayNumber - checkIn.DayNumber;
        decimal total = _repo.CalculateTotalPrice(room, checkIn, checkOut);
        var sv = new CultureInfo("sv-SE");

        Console.WriteLine();
        Console.WriteLine($"Bokning bekräftad! Totalt: {total.ToString("N0", sv)} kr ({nights} nätter × {room.PricePerNight.ToString("N0", sv)} kr)");
    }

    private bool ShowAllBookings()
    {
        var bookings = _repo.GetAllBookings();

        if (bookings.Count == 0)
        {
            Console.WriteLine("Det finns inga bokningar.");
            return false;
        }

        Console.WriteLine("Alla bokningar:");
        var sv = new CultureInfo("sv-SE");

        foreach (Booking b in bookings)
        {
            int nights = b.CheckOut.DayNumber - b.CheckIn.DayNumber;
            decimal total = _repo.CalculateTotalPrice(b.Room, b.CheckIn, b.CheckOut);

            Console.WriteLine($"[{b.Id}] {b.Guest.Name}, rum {b.Room.RoomNumber}, {b.CheckIn:yyyy-MM-dd} till {b.CheckOut:yyyy-MM-dd} ({nights} nätter, {total.ToString("N0", sv)} kr)");
        }

        return true;
    }

    private void CancelBooking()
    {
        Console.WriteLine("--- Avboka ---");

        if (!ShowAllBookings())
        {
            return;
        }

        Console.WriteLine();
        Console.Write("Boknings-id att avboka: ");
        string? text = Console.ReadLine();

        if (!int.TryParse(text, out int bookingId))
        {
            Console.WriteLine("Id måste vara ett tal.");
            return;
        }

        if (_repo.CancelBooking(bookingId))
        {
            Console.WriteLine("Bokningen är avbokad.");
        }
        else
        {
            Console.WriteLine($"Ingen bokning med id {bookingId} hittades.");
        }
    }

    private DateOnly ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? text = Console.ReadLine();

            if (DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
            {
                return date;
            }

            Console.WriteLine("Ogiltigt datum. Skriv så här: 2026-12-01");
        }
    }
    private void SearchAvailableRooms()
    {
        Console.WriteLine("\n--- Sök efter lediga rum ---");

        DateOnly from = ReadDate("Ange incheckningsdatum (yyyy-MM-dd): ");
        DateOnly to = ReadDate("Ange utcheckningsdatum (yyyy-MM-dd): ");

        if (to <= from)
        {
            Console.WriteLine("Utcheckningsdatum måste vara efter incheckningsdatum.");
            return;
        }

        List<Rooms> availableRooms = _repo.GetAvailableRooms(from, to);

        if (availableRooms.Count == 0)
        {
            Console.WriteLine("Det finns inga lediga rum under den perioden.");
            return;
        }

        Console.WriteLine("\nLediga rum:");

        foreach (var room in availableRooms)
        {
            Console.WriteLine(
                $"Rum {room.RoomNumber} | " +
                $"{room.RoomType.Name} | " +
                $"{room.SizeM2} m² | " +
                $"{room.PricePerNight:N0} kr/natt");
        }
    }
}