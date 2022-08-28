using RoomBookingApp.Domain;

namespace RoomBookingApp.Core.Services;

public interface IRoomBookingService
{
    void Save(RoomBooking roomBooking);
    IEnumerable<Room> GetAvailableRooms(DateTime date);
}
