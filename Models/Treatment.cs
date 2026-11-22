public class Treatment
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int DurationMinutes { get; set; }

    // Priset är decimal precis som på rummen. SQLite lagrar det som text,
    // så vi sorterar aldrig på pris i databasen.
    public decimal Price { get; set; }

    // En behandling kan bokas många gånger
    public List<SpaBooking> SpaBookings { get; set; } = new();
}