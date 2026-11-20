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

        _repo.AddBooking(name, room, checkIn, checkOut);

        int nights = checkOut.DayNumber - checkIn.DayNumber;
        decimal total = _repo.CalculateTotalPrice(room, checkIn, checkOut);
        var sv = new CultureInfo("sv-SE");

        Console.WriteLine();
        Console.WriteLine($"Bokning bekräftad! Totalt: {total.ToString("N0", sv)} kr ({nights} nätter × {room.PricePerNight.ToString("N0", sv)} kr)");
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
}