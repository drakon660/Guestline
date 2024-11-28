using FluentAssertions;
using Guestline.Core;
using Moq;

namespace Guestline.Tests;

public class CheckAvailabilityTests
{
    [Fact]
    public void Check_If_Room_Is_Available_Before_Bookings()
    {
        Booking[] bookings =
        [
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 04))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 06), new DateOnly(2024, 01, 07)))
        ];

        Hotel hotel = HotelMother.TwoSGLTwoDBL(bookings);

        var requestedDateRange = DateRange.Create(new DateOnly(2023, 12, 29), new DateOnly(2023, 12, 31));

        int count = hotel.CheckAvailability(RoomType.Single, requestedDateRange);
        count.Should().Be(2);
    }

    [Fact]
    public void Check_If_Room_Is_Available_After_Bookings()
    {
        Booking[] bookings =
        [
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 04))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 06), new DateOnly(2024, 01, 07)))
        ];

        Hotel hotel = HotelMother.TwoSGLTwoDBL(bookings);

        var requestedDateRange = DateRange.Create(new DateOnly(2024, 01, 08), new DateOnly(2024, 01, 18));

        int count = hotel.CheckAvailability(RoomType.Single, requestedDateRange);
        count.Should().Be(2);
    }

    [Fact]
    public void Check_If_Room_Is_Not_Available_When_Bookings_Overlap()
    {
        Booking[] bookings =
        [
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 02), new DateOnly(2024, 01, 04)))
        ];

        Hotel hotel = HotelMother.TwoSGLTwoDBL(bookings);

        var requestedDateRange = DateRange.Create(new DateOnly(2024, 01, 03), new DateOnly(2024, 01, 04));

        int count = hotel.CheckAvailability(RoomType.Single, requestedDateRange);
        count.Should().Be(0);
    }

    [Fact]
    public void Check_If_Double_Is_Free_When_All_Single_Are_Booked()
    {
        Booking[] bookings =
        [
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03)))
        ];

        Hotel hotel = HotelMother.DynamicRoomsAndBookings(bookings, [
            Room.Single("101"),
            Room.Single("102"),
            Room.Single("103"),
            Room.Double("201"),
        ]);

        var requestedDateRange = DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03));

        int count = hotel.CheckAvailability(RoomType.Double, requestedDateRange);
        count.Should().Be(1);
    }

    [Fact]
    public void Check_If_Single_3_Room_Is_Available()
    {
        Booking[] bookings =
        [
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03))), 
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 04))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 06)))
        ];
        
        Hotel hotel = GetHotel(bookings);

        var requestedDateRange = DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 07));

        int count = hotel.CheckAvailability(RoomType.Single, requestedDateRange);
        count.Should().Be(1);
    }

    [Fact]
    public void Check_Availability_Should_Return_Zero_When_Room_Type_Not_Found()
    {
        Booking[] bookings =
        [
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 02), new DateOnly(2024, 01, 05))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 06))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 06), new DateOnly(2024, 01, 08)))
        ];

        var hotel = HotelMother.DynamicRoomsAndBookings(bookings, [
            Room.Single("101"),
            Room.Single("102"),
            Room.Single("103"),
        ]);

        var requestedDateRange = DateRange.Create(new DateOnly(2024, 01, 08), new DateOnly(2024, 01, 15));
        int count = hotel.CheckAvailability(RoomType.Double, requestedDateRange);

        count.Should().Be(0);
    }

    [Fact]
    public void Check_Availability_Should_Fail_When_Bookings_Overlap_Second()
    {
        var bookings = new[]
        {
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 02), new DateOnly(2024, 01, 05))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 06))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 06), new DateOnly(2024, 01, 08)))
        };

        var hotel = GetHotel(bookings);

        var requestedDateRange = DateRange.Create(new DateOnly(2024, 01, 07), new DateOnly(2024, 01, 08));
        int count = hotel.CheckAvailability(RoomType.Single, requestedDateRange);

        count.Should().Be(2);
    }

    private Hotel GetHotel(Booking[] bookings)
    {
        Room sgl101 = Room.Create("101", "SGL");
        Room sgl102 = Room.Create("102", "SGL");
        Room dbl201 = Room.Create("201", "DBL");
        Room dbl202 = Room.Create("202", "DBL");
        Room sgl103 = Room.Create("103", "SGL");
        
        return Hotel.Create("H1", "Verona", [sgl101, sgl102, dbl201, dbl202, sgl103], bookings);
    }
}