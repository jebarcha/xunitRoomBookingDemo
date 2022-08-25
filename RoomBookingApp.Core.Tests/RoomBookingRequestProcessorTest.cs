using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Shouldly;

namespace RoomBookingApp.Core;

public class RoomBookingRequestProcessorTest
{
    [Fact]
    public void Should_Return_Room_Booking_Request()
    {
        //Arrange
        var request = new RoomBookingRequest
        {
            FullName = "Test Name",
            Email = "test@email.com",
            Date = new DateTime(2022, 9, 25)
        };

        var processor = new RoomBookingRequestProcessor();

        //Act
        RoomBookingResult result = processor.BookRoom(request);

        //Assert
        //Assert.NotNull(result);
        //Assert.Equal(request.FullName, result.FullName);
        //Assert.Equal(request.Email, result.Email);
        //Assert.Equal(request.Date, result.Date);

        //fluent assertions (Shoudly)
        result.ShouldNotBeNull("result should not be null");
        result.FullName.ShouldBe(request.FullName);
        result.Email.ShouldBe(request.Email);
        result.Date.ShouldBe(request.Date);
    }
}
