public class Guest
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    // E-post och telefon är valfria (string?) eftersom personalen frågar men
    // gästen inte måste lämna dem. Se GDPR-avsnittet i README.
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    // En gäst kan ha många bokningar och många spabokningar
    public List<Booking> Bookings { get; set; } = new();
    public List<SpaBooking> SpaBookings { get; set; } = new();
}