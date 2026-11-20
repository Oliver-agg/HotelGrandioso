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
            Console.WriteLine("== Hotel Grandioso ==");
            Console.WriteLine();
            Console.WriteLine("1. Visa alla rum");
            Console.WriteLine();
            Console.WriteLine("2. Avsluta");
            Console.WriteLine();
            Console.Write("Val: ");
            string? choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    ShowAllRooms();
                    break;
                case "2":
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
}
