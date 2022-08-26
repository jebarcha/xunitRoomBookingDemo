using RoomBookingApp.Core.Domain;

namespace RoomBookingApp.Core.Services;

public interface IBookingService
{
    void Save(RoomBooking roomBooking);
    IEnumerable<Room> GetAvailableRooms(DateTime date);
}
