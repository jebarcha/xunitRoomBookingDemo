using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using RoomBookingApp.Domain;
using RoomBookingApp.Persistence.Repositories;
using Shouldly;

namespace RoomBookingApp.Persistence.Tests;

public class RoomBookingServiceTest
{
    [Fact]
    public void Should_Return_Available_Rooms()
    {
        //Arrange
        var date = new DateTime(2022, 08, 28);

        var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
            .UseInMemoryDatabase("AvailabeRoomTest")
            .Options;

        using var context = new RoomBookingAppDbContext(dbOptions);
        context.Add(new Room { Id = 1, Name = "Room 1" });
        context.Add(new Room { Id = 2, Name = "Room 2" });
        context.Add(new Room { Id = 3, Name = "Room 3" });

        context.Add(new RoomBooking { RoomId = 1, Date = date });
        context.Add(new RoomBooking { RoomId = 2, Date = date.AddDays(-1) });

        context.SaveChanges();

        var roomBookingService = new RoomBookingService(context);

        //Act
        var availableRooms = roomBookingService.GetAvailableRooms(date);
        context.Dispose();

        //Assert
        availableRooms.Count().ShouldBeGreaterThan(0);
        
        //availableRooms.ShouldContain(q => q.Id == 2);     //Shouldly
        availableRooms.Should().Contain(q => q.Id == 2);  //FluentAssertion

        availableRooms.ShouldContain(q => q.Id == 3);
        availableRooms.ShouldNotContain(q => q.Id == 1);

        //xUnit Assert
        //Assert.Equal(2, availableRooms.Count());
        //Assert.Contains(availableRooms, q => q.Id == 2);
        //Assert.Contains(availableRooms, q => q.Id == 3);
        //Assert.DoesNotContain(availableRooms, q => q.Id == 1);
    }

    [Fact]
    public void Should_Save_Room_Booking()
    {
        var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
            .UseInMemoryDatabase("AvailabeRoomTest")
            .Options;

        var roomBooking = new RoomBooking { RoomId = 1, Date = new DateTime(2022, 8, 28) };

        using var context = new RoomBookingAppDbContext(dbOptions);
        var roomBookingService = new RoomBookingService(context);
        roomBookingService.Save(roomBooking);

        var bookings = context.RoomBookings.ToList();
        var booking = Assert.Single(bookings);

        Assert.Equal(roomBooking.Date, booking.Date);
        Assert.Equal(roomBooking.RoomId, booking.RoomId);
    }

    [Fact]
    public void Should_ThrowError_IfRoomBookingIsNull()
    {
        var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
            .UseInMemoryDatabase("AvailabeRoomTest")
            .Options;
        using var context = new RoomBookingAppDbContext(dbOptions);
        var roomBookingService = new RoomBookingService(context);
        var exception = Should.Throw<ArgumentNullException>(() => roomBookingService.Save(null));
        exception.ParamName.ShouldBe("roomBooking");
    }
}
