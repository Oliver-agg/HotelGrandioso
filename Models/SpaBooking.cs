// SpaBooking fungerar som Booking, men kopplar ihop en gäst med en
// behandling ett visst datum i stället för ett rum under en period
public class SpaBooking
{
    public int Id { get; set; }

    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;

    public int TreatmentId { get; set; }
    public Treatment Treatment { get; set; } = null!;

    // Bara datum, inte klockslag. Uppgiften kräver inte mer.
    public DateOnly Date { get; set; }
}