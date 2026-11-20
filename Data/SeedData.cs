public static class SeedData
{
    public static void Seed(HotelContext db)
    {
        if (db.RoomTypes.Any()) return;

        var standard = new RoomType { Name = "Standard", Description = "Standard Room" };
        var luxury = new RoomType { Name = "Luxury", Description = "Luxurious Room" };
        var bridal = new RoomType { Name = "Bridal", Description = "Bridal Room" };

        db.RoomTypes.Add(standard);
        db.RoomTypes.Add(luxury);
        db.RoomTypes.Add(bridal);

        db.Rooms.Add(new Rooms
        {
            RoomNumber = 304,
            Floor = 3,
            SizeM2 = 180,
            PricePerNight = 2800,
            RoomType = luxury
        });

        db.Rooms.Add(new Rooms
        {
            RoomNumber = 305,
            Floor = 3,
            SizeM2 = 220,
            PricePerNight = 3500,
            RoomType = bridal
        });

        for (int number = 101; number <= 105; number++)
        {
            db.Rooms.Add(new Rooms
            {
                RoomNumber = number,
                Floor = 1,
                SizeM2 = 80,
                PricePerNight = 800,
                RoomType = standard
            });
        }

        for (int number = 201; number <= 205; number++)
        {
            db.Rooms.Add(new Rooms
            {
                RoomNumber = number,
                Floor = 2,
                SizeM2 = 100,
                PricePerNight = 1000,
                RoomType = standard
            });
        }

        for (int number = 301; number <= 303; number++)
        {
            db.Rooms.Add(new Rooms
            {
                RoomNumber = number,
                Floor = 3,
                SizeM2 = 120,
                PricePerNight = 1200,
                RoomType = standard
            });
        }

        db.SaveChanges();
    }
}
