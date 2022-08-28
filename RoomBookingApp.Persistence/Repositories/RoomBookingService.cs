using RoomBookingApp.Core.Services;
using RoomBookingApp.Domain;

namespace RoomBookingApp.Persistence.Repositories;

public class RoomBookingService : IRoomBookingService
{
    private readonly RoomBookingAppDbContext _context;

    public RoomBookingService(RoomBookingAppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Room> GetAvailableRooms(DateTime date)
    {
        return _context.Rooms.Where(q => !q.RoomBookings.Any(x => x.Date == date)).ToList();

        //var unAvailableRooms = _context.RoomBookings.Where(q => q.Date == date)
        //    .Select(q => q.RoomId).ToList();
        //var availableRooms = _context.Rooms.Where(q => !unAvailableRooms.Contains(q.Id));
        //return availableRooms;
    }

    public void Save(RoomBooking roomBooking)
    {
        if (roomBooking is null)
        {
            throw new ArgumentNullException(nameof(roomBooking));
        }

        _context.Add(roomBooking);
        _context.SaveChanges();
    }
}
