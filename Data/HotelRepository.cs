using Microsoft.EntityFrameworkCore;
public class HotelRepository
{
    private readonly HotelContext _db;
    public HotelRepository(HotelContext db)
    {
        _db = db;
    }

    public List<Rooms> GetAllRooms()
    {
        return _db.Rooms
            .Include(r => r.RoomType)
            .OrderBy(r => r.RoomNumber)
            .ToList();
    }
}