// Seed-data är startdata: hotellets rum och spabehandlingar finns från dag
// ett, så vi lägger in dem med kod i stället för att någon ska göra det för
// hand.
public static class SeedData
{
    public static void Seed(HotelContext db)
    {
        // Behandlingarna har en egen kontroll och körs först. Annars skulle de
        // aldrig läggas in i en databas som redan har rum.
        SeedTreatments(db);

        // Skyddet gör att rummen bara läggs in första gången. Utan det skulle
        // de dubbleras varje gång programmet startas.
        if (db.RoomTypes.Any()) return;

        var standard = new RoomType { Name = "Standard", Description = "Standard Room" };
        var luxury = new RoomType { Name = "Luxury", Description = "Luxurious Room" };
        var bridal = new RoomType { Name = "Bridal", Description = "Bridal Room" };

        // Rumstyperna skapas före rummen, eftersom varje rum pekar på en typ
        db.RoomTypes.Add(standard);
        db.RoomTypes.Add(luxury);
        db.RoomTypes.Add(bridal);

        // 304 och 305 är enstaka rum med egen typ och eget pris, så de skrivs
        // som egna block. Vi sätter hela typobjektet och inte ett id, så fyller
        // EF i RoomTypeId själv.
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

        // Rummen på en våning är likadana och skiljer sig bara på numret,
        // så en loop är kortare än ett block per rum.
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

        // Bara 301-303: rum 304 och 305 är redan skapade ovan
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

        // Vi sparar en enda gång sist. Allt läggs då in tillsammans, och går
        // något fel sparas ingenting, så vi får aldrig en halv seed.
        db.SaveChanges();
    }

    private static void SeedTreatments(HotelContext db)
    {
        // Samma skydd som för rummen: lägg bara in behandlingarna en gång
        if (db.Treatments.Any()) return;

        // Uppgiften anger bara tiden för massage (60 min). Entré är "per
        // timme", alltså 60 minuter. Tiderna för pedikyr är mina antaganden.
        db.Treatments.Add(new Treatment { Name = "Entré (per timme)", DurationMinutes = 60, Price = 150 });
        db.Treatments.Add(new Treatment { Name = "Massage", DurationMinutes = 60, Price = 950 });
        db.Treatments.Add(new Treatment { Name = "Pedikyr (stortån)", DurationMinutes = 30, Price = 350 });
        db.Treatments.Add(new Treatment { Name = "Pedikyr (hel fot)", DurationMinutes = 60, Price = 650 });

        db.SaveChanges();
    }
}