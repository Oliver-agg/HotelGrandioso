using Microsoft.EntityFrameworkCore;

// Utan det här kan å, ä, ö och ² bli frågetecken i konsolen
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

// using gör att databasen stängs av sig själv när programmet är klart
using var db = new HotelContext();

// Migrate skapar databasen och tabellerna, så att ingen behöver köra
// dotnet ef database update själv
db.Database.Migrate();

// Lägger in rum och behandlingar första gången, annars händer ingenting
SeedData.Seed(db);

// Ordningen är databas -> repository -> meny. Varje klass får det den
// behöver utifrån, så att den bara har ett ansvar.
var repo = new HotelRepository(db);
var menu = new HotelMenu(repo);
menu.Run();