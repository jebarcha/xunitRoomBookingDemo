using Moq;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using RoomBookingApp.Core.Services;
using RoomBookingApp.Domain;
using Shouldly;
using Xunit.Abstractions;

namespace RoomBookingApp.Core;

public class RoomBookingRequestProcessorTest
{
    private readonly ITestOutputHelper output;
    private readonly RoomBookingRequestProcessor _processor;
    private readonly RoomBookingRequest _request;
    private readonly Mock<IRoomBookingService> _roomBookingServiceMock;
    private List<Room> _availableRooms;

    public RoomBookingRequestProcessorTest(ITestOutputHelper output)
    {
        this.output = output;
        
        //Arrange
        _request = new RoomBookingRequest
        {
            FullName = "Test Name",
            Email = "test@email.com",
            Date = new DateTime(2022, 9, 25)
        };
        _availableRooms = new List<Room>() { new Room() { Id = 1 } };

        _roomBookingServiceMock = new Mock<IRoomBookingService>();
        _roomBookingServiceMock.Setup(q => q.GetAvailableRooms(_request.Date))
            .Returns(_availableRooms);

        _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);
    }

    [Fact]
    public void Should_Return_Room_Booking_Request()
    {
        //Act
        RoomBookingResult result = _processor.BookRoom(_request);

        //Assert
        //Assert.NotNull(result);
        //Assert.Equal(request.FullName, result.FullName);
        //Assert.Equal(request.Email, result.Email);
        //Assert.Equal(request.Date, result.Date);

        //fluent assertions (Shoudly)
        result.ShouldNotBeNull("result should not be null");
        result.FullName.ShouldBe(_request.FullName);
        result.Email.ShouldBe(_request.Email);
        result.Date.ShouldBe(_request.Date);
    }

    [Fact]
    public void Should_Throw_Exception_For_Null_Request()
    {
        var exception = Should.Throw<ArgumentNullException>(() => _processor.BookRoom(null));

        output.WriteLine($"exception.ParamName: {exception.ParamName}");
        exception.ParamName.ShouldBe("request");
    }

    [Fact]
    public void Should_Save_Room_Booking_Request()
    {
        RoomBooking savedBooking = null;
        _roomBookingServiceMock.Setup(q => q.Save(It.IsAny<RoomBooking>()))
            .Callback<RoomBooking>(booking =>
            {
                savedBooking = booking;
            });

        _processor.BookRoom(_request);

        _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Once);

        savedBooking.ShouldNotBeNull();
        savedBooking.FullName.ShouldBe(_request.FullName);
        savedBooking.Email.ShouldBe(_request.Email);
        savedBooking.Date.ShouldBe(_request.Date);
        savedBooking.RoomId.ShouldBe(_availableRooms.First().Id);
    }

    [Fact]
    public void Should_Not_Save_Room_Booking_Request_If_Non_Available()
    {
        _availableRooms.Clear();
        _processor.BookRoom(_request);
        _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Never);
    }

    [Theory]
    [InlineData(BookingResultFlag.Failure, false)]
    [InlineData(BookingResultFlag.Success, true)]
    public void Should_Return_SuccessFailure_Flag_In_Result(BookingResultFlag bookingSuccessFlag, bool isAvailable)
    {
        if (!isAvailable)
        {
            _availableRooms.Clear();
        }

        var result = _processor.BookRoom(_request);
        bookingSuccessFlag.ShouldBe(result.Flag);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(null, false)]
    public void Should_Return_RoomBookingId_In_Result(int? roomBookingId, bool isAvailable)
    {
        if (!isAvailable)
        {
            _availableRooms.Clear();
        }
        else 
        {
            _roomBookingServiceMock.Setup(q => q.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(booking =>
                {
                    booking.Id = roomBookingId.Value;
                });
        }

        var result = _processor.BookRoom(_request);
        result.RoomBookingId.ShouldBe(roomBookingId);
    }
}
