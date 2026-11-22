using System.Globalization;

// Menyn sköter allt som skrivs ut och läses in i konsolen. Den använder bara
// repositoryt och pratar aldrig direkt med databasen.
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

        // Loopen gör att menyn kommer tillbaka efter varje val,
        // tills jag väljer 0
        while (running)
        {
            Console.WriteLine("=== Hotel Grandioso ===");
            Console.WriteLine();
            Console.WriteLine("1. Boka rum");
            Console.WriteLine("2. Visa alla rum");
            Console.WriteLine("3. Visa bokningar");
            Console.WriteLine("4. Avboka");
            Console.WriteLine("5. Sök efter lediga rum");
            Console.WriteLine("6. Boka spa");
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
                case "6":
                    BookSpa();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    // Ett felmeddelande i stället för en krasch om man
                    // skriver något som inte är ett val
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
            // :0 visar priset som ett heltal, utan decimaler
            Console.WriteLine($"{room.RoomNumber} - {room.RoomType.Name}  {room.SizeM2} m²  {room.PricePerNight:0} kr/natt");
        }
    }

    private void BookRoom()
    {
        Console.WriteLine("--- Boka rum ---");

        // Ett tomt namn är meningslöst, så jag avbryter direkt. Varje return i
        // den här metoden gör att ingenting sparas när något är fel.
        Console.Write("Gästnamn: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Namnet får inte vara tomt.");
            return;
        }

        // E-post och telefon är valfria. Personalen frågar, men gästen måste
        // inte lämna dem. Ett tomt svar blir null så att det inte sparas som
        // en tom text.
        Console.Write("E-post (valfritt, tryck Enter för att hoppa över): ");
        string? email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email)) email = null;

        Console.Write("Telefonnummer (valfritt, tryck Enter för att hoppa över): ");
        string? phone = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(phone)) phone = null;

        // ReadDate frågar om tills datumet är giltigt
        DateOnly checkIn = ReadDate("Incheckning (YYYY-MM-DD): ");
        DateOnly checkOut = ReadDate("Utcheckning (YYYY-MM-DD): ");

        // Utcheckningen måste vara efter incheckningen, annars finns inga nätter
        if (checkOut <= checkIn)
        {
            Console.WriteLine("Utcheckningen måste vara efter incheckningen.");
            return;
        }

        // Jag visar rummen så att man ser vad man kan välja
        Console.WriteLine();
        ShowAllRooms();
        Console.WriteLine();

        // TryParse gör att bokstäver inte kraschar programmet
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

        // AddBooking ger null om rummet redan är bokat de datumen. Då avbryter
        // jag, så att ingen bekräftelse skrivs ut.
        Booking? booking = _repo.AddBooking(name, room, checkIn, checkOut, email, phone);
        if (booking == null)
        {
            Console.WriteLine($"Rum {roomNumber} är redan bokat de datumen.");
            return;
        }

        int nights = checkOut.DayNumber - checkIn.DayNumber;
        decimal total = _repo.CalculateTotalPrice(room, checkIn, checkOut);
        var sv = new CultureInfo("sv-SE");

        // N0 med svensk kultur ger mellanrum i stora tal, så det blir 10 500
        Console.WriteLine();
        Console.WriteLine($"Bokning bekräftad! Totalt: {total.ToString("N0", sv)} kr ({nights} nätter × {room.PricePerNight.ToString("N0", sv)} kr)");
    }

    // Svarar true om det fanns bokningar att visa. Avboka använder det för att
    // slippa fråga efter ett id när listan är tom.
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

            // Id visas först eftersom man behöver det för att avboka
            Console.WriteLine($"[{b.Id}] {b.Guest.Name}, rum {b.Room.RoomNumber}, {b.CheckIn:yyyy-MM-dd} till {b.CheckOut:yyyy-MM-dd} ({nights} nätter, {total.ToString("N0", sv)} kr)");
        }

        return true;
    }

    private void CancelBooking()
    {
        Console.WriteLine("--- Avboka ---");

        // Jag visar bokningarna först så att man ser vilka id som finns
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
        // Loopen fortsätter tills man skriver ett giltigt datum
        while (true)
        {
            Console.Write(prompt);
            string? text = Console.ReadLine();

            // TryParseExact kräver exakt formatet yyyy-MM-dd och kraschar inte
            // om det blir fel. InvariantCulture gör att det funkar på alla
            // datorer oavsett språkinställning.
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

        // Samma kontroll som vid bokning: det måste vara minst en natt
        if (to <= from)
        {
            Console.WriteLine("Utcheckningsdatum måste vara efter incheckningsdatum.");
            return;
        }

        // Själva sökningen görs i repositoryt. Menyn visar bara resultatet.
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

    private void BookSpa()
    {
        Console.WriteLine("--- Boka spa ---");

        Console.Write("Gästnamn: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Namnet får inte vara tomt.");
            return;
        }

        // Jag visar behandlingarna så att man ser vad man kan välja
        Console.WriteLine();
        Console.WriteLine("Behandlingar:");
        foreach (Treatment t in _repo.GetAllTreatments())
        {
            Console.WriteLine($"{t.Id} - {t.Name}  {t.DurationMinutes} min  {t.Price:0} kr");
        }
        Console.WriteLine();

        // TryParse gör att bokstäver inte kraschar programmet
        Console.Write("Välj behandling (nummer): ");
        string? text = Console.ReadLine();
        if (!int.TryParse(text, out int treatmentId))
        {
            Console.WriteLine("Numret måste vara ett tal.");
            return;
        }

        Treatment? treatment = _repo.GetTreatmentById(treatmentId);
        if (treatment == null)
        {
            Console.WriteLine($"Behandling {treatmentId} finns inte.");
            return;
        }

        DateOnly date = ReadDate("Datum (YYYY-MM-DD): ");

        // Allt är kontrollerat, så nu sparar jag spabokningen
        _repo.AddSpaBooking(name, treatment, date);

        Console.WriteLine();
        Console.WriteLine($"Spabokning bekräftad! {treatment.Name} den {date:yyyy-MM-dd} för {name}, {treatment.Price:0} kr");
    }
}