using Microsoft.EntityFrameworkCore;

using var db = new HotelContext();
db.Database.Migrate();
SeedData.Seed(db);

Console.WriteLine("Databasen är klar.");