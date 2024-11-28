using FluentAssertions;
using Guestline.Core;
using Guestline.Core.Services;
using Moq;

namespace Guestline.Tests;

public class BookingServiceTests
{
    [Fact]
    public async Task Check_If_BookingService_Is_Returning_Range()
    {
        var bookingRepositoryMock = new Mock<IBookingRepository>();
        var dateProviderMock = new Mock<IDateProvider>();
    
        Booking[] bookings =
        [
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 04))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 06), new DateOnly(2024, 01, 07)))
        ];
    
        bookingRepositoryMock.Setup(x => x.GetHotelById("H1")).ReturnsAsync(HotelMother.TwoSGLTwoDBL(bookings));
        dateProviderMock.Setup(x=>x.GetCurrentDate()).Returns(new DateOnly(2023, 12, 31));
        
        var bookingService = new BookingService(bookingRepositoryMock.Object, dateProviderMock.Object);
    
        var availability = await bookingService.CheckAvailability("H1", "20240101-20240104", RoomType.Single);
        availability.Should().Be("(20240101-20240104), 1");
    }
}