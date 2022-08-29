using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Services;
using RoomBookingApp.Domain.Models;
using RoomBookingApp.Domain;

namespace RoomBookingApp.Core.Processors;

public class RoomBookingRequestProcessor : IRoomBookingRequestProcessor
{
    private readonly IRoomBookingService _bookingService;

    public RoomBookingRequestProcessor(IRoomBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public RoomBookingResult BookRoom(RoomBookingRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var availableRooms = _bookingService.GetAvailableRooms(request.Date);
        var result = CreateRoomBookingObject<RoomBookingResult>(request);

        if (availableRooms.Any())
        {
            var room = availableRooms.First();
            var roomBooking = CreateRoomBookingObject<RoomBooking>(request);
            roomBooking.RoomId = room.Id;
            _bookingService.Save(roomBooking);

            result.RoomBookingId = roomBooking.Id;
            result.Flag = Enums.BookingResultFlag.Success;
        }
        else
        {
            result.Flag = Enums.BookingResultFlag.Failure;
        }

        return result;
    }

    private static TRoomBooking CreateRoomBookingObject<TRoomBooking>(RoomBookingRequest request)
        where TRoomBooking : RoomBookingBase, new()
    {
        return new TRoomBooking
        {
            FullName = request.FullName,
            Email = request.Email,
            Date = request.Date
        };
    }
}