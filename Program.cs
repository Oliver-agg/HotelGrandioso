using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

using var db = new HotelContext();

db.Database.Migrate();

SeedData.Seed(db);

var repo = new HotelRepository(db);
var menu = new HotelMenu(repo);
menu.Run();