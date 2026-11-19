public class Booking
{
    public int Id { get; set; }

    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;

    public int RoomId { get; set; }
    public Rooms Room { get; set; } = null!;

    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
}