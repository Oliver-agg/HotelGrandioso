// Booking kopplar ihop en gäst med ett rum under en period. En gäst kan
// boka många rum och ett rum kan bokas av många gäster, så det behövs en
// tabell emellan. Den har också egna uppgifter, nämligen datumen.
public class Booking
{
    public int Id { get; set; }

    // GuestId sparas i databasen, Guest är gästen som objekt i koden
    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;

    // RoomId sparas i databasen, Room är rummet som objekt i koden
    public int RoomId { get; set; }
    public Rooms Room { get; set; } = null!;

    // Utcheckningsdagen räknas inte som en upptagen natt
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
}