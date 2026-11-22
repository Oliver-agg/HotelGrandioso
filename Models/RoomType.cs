public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    // En rumstyp har många rum. Typen sparas en gång i stället för att
    // upprepas på varje rum.
    public List<Rooms> Rooms { get; set; } = new();
}