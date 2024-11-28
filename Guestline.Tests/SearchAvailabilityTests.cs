using FluentAssertions;
using Guestline.Core;

namespace Guestline.Tests;

public class SearchAvailabilityTests
{
    [Fact]
    public void SearchAvailability_None()
    {
        var bookings = new[]
        {
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 02), new DateOnly(2024, 01, 05))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 06))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 06), new DateOnly(2024, 01, 08)))
        };

        var hotel = HotelMother.TwoSGLTwoDBL(bookings);

        var availability = hotel.SearchAvailability(RoomType.Single, 365, 3, () => new DateOnly(2023, 01, 04));
        availability.Should().BeEmpty();
    }

    [Fact]
    public void SearchAvailability_Multiple()
    {
        var bookings = new[]
        {
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 02), new DateOnly(2024, 01, 05))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 06))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 06), new DateOnly(2024, 01, 08)))
        };
        
        var hotel =  HotelMother.TwoSGLTwoDBL(bookings);

        var availability = hotel.SearchAvailability(RoomType.Single, 365, 14, () => new DateOnly(2022, 12, 28));
        availability.Should().BeEquivalentTo(new List<(DateRange DateRange, int Rooms )>
        {
            new(DateRange.FromDayNumber([738881, 738882, 738883, 738884]), 2),
            new(DateRange.FromDayNumber([738885]), 1),
            new(DateRange.FromDayNumber([738891, 738892]), 1),
            new(DateRange.FromDayNumber([738893, 738894]), 2),
        });
    }

    [Fact]
    public void SearchAvailability_Another_None()
    {
        var bookings = new[]
        {
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 02), new DateOnly(2024, 01, 05))),
            Booking.SGL( DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 06))),
            Booking.SGL(DateRange.Create(new DateOnly(2024, 01, 06), new DateOnly(2024, 01, 08)))
        };

        var hotel = HotelMother.TwoSGLTwoDBL(bookings);

        var availability = hotel.SearchAvailability(RoomType.Single, 365, 3, () => new DateOnly(2023, 01, 04));
        availability.Should().BeEmpty();
    }

    [Fact]
    public void SearchAvailability_DynamicRangeWithBookings()
    {
        var bookings = new[]
        {
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 02))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 05))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 06), new DateOnly(2024, 01, 07))) // Double booking for 2024-01-06 to 2024-01-07
        };
        
        var hotel = HotelMother.TwoSGLTwoDBL(bookings);
        
        var currentDate = new DateOnly(2024, 01, 01); // Starting date for the search
        int daysCountAheadOfSearch = 3;
        int daysCount = 5; 

        
        var availability = hotel.SearchAvailability(RoomType.Double, daysCountAheadOfSearch, daysCount, () => currentDate);
        
        availability.Should().BeEquivalentTo(new List<(DateRange DateRange, int Rooms)>
        {
            (DateRange.FromDayNumber([738888, 738889, 738890, 738891]), 1), 
            (DateRange.FromDayNumber([738892]), 2)
        });
    }
    
    [Fact]
    public void SearchAvailability_ComplexBookingScenario_WithZeroAvailability()
    {
        var bookings = new[]
        {
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 02))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 05))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 06), new DateOnly(2024, 01, 07))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 07), new DateOnly(2024, 01, 08))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 09), new DateOnly(2024, 01, 10))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 08), new DateOnly(2024, 01, 10))),
        };
        
        var hotel = HotelMother.DynamicRoomsAndBookings(bookings.ToArray(), [
            Room.Create("101", "SGL"),
            Room.Create("102", "SGL"),
            Room.Create("201", "DBL"),
            Room.Create("202", "DBL"),
            Room.Create("203", "DBL"),
            Room.Create("204", "DBL"),
        ]);
        
        var currentDate = new DateOnly(2024, 01, 01);
        int daysCountAheadOfSearch = 3; 
        int daysCount = 7; 

        var availability = hotel.SearchAvailability(RoomType.Double, daysCountAheadOfSearch, daysCount, () => currentDate);
        
        availability.Should().BeEquivalentTo(new List<(DateRange DateRange, int Rooms)>
        {
            (DateRange.FromDayNumber([738888, 738889, 738890]), 3),
            (DateRange.FromDayNumber([738891, 738892, 738893, 738894]), 2)
        });
    }
    
    [Fact]
    public void SearchAvailability_Fragile()
    {
        var bookings = new[]
        {
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 02))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 02))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 03))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 07))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 03), new DateOnly(2024, 01, 04))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 07), new DateOnly(2024, 01, 07))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 04), new DateOnly(2024, 01, 04))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 07), new DateOnly(2024, 01, 07))),
        };
        
        var hotel = HotelMother.DynamicRoomsAndBookings(bookings.ToArray(), [

            Room.Create("201", "DBL"),
            Room.Create("202", "DBL"),
            Room.Create("203", "DBL"),
        ]);
        
        var currentDate = new DateOnly(2024, 01, 01);
        int daysCountAheadOfSearch = 0; 
        int daysCount = 7; 

        var availability = hotel.SearchAvailability(RoomType.Double, daysCountAheadOfSearch, daysCount, () => currentDate);
        
        availability.Should().BeEquivalentTo(new List<(DateRange DateRange, int Rooms)>
        {
            (DateRange.FromDayNumber([738887]), 1),
            (DateRange.FromDayNumber([738889, 738890]), 2)
        });
    }
    [Fact]
    public void SearchAvailability_OneDay_Not_Booked()
    {
        var bookings = new[]
        {
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 01))),
        };
        
        var hotel = HotelMother.DynamicRoomsAndBookings([], [

            Room.Create("201", "DBL"),
        ]);
        
        var currentDate = new DateOnly(2024, 01, 01);
        int daysCountAheadOfSearch = 0; 
        int daysCount = 1; 

        var availability = hotel.SearchAvailability(RoomType.Double, daysCountAheadOfSearch, daysCount, () => currentDate);
        
        availability.Should().BeEquivalentTo(new List<(DateRange DateRange, int Rooms)>
        {
            (DateRange.FromDayNumber([738885]), 1),
        });
    }
    
    [Fact]
    public void SearchAvailability_TwoDays_Not_Booked()
    {
        var hotel = HotelMother.DynamicRoomsAndBookings([], [

            Room.Create("201", "DBL"),
        ]);
        
        var currentDate = new DateOnly(2024, 01, 01);
        int daysCountAheadOfSearch = 0; 
        int daysCount = 2; 

        var availability = hotel.SearchAvailability(RoomType.Double, daysCountAheadOfSearch, daysCount, () => currentDate);
        
        availability.Should().BeEquivalentTo(new List<(DateRange DateRange, int Rooms)>
        {
            (DateRange.FromDayNumber([738885,738886]), 1),
        });
    }
    
    [Fact]
    public void SearchAvailability_ThreeDays_Not_Booked()
    {
        var hotel = HotelMother.DynamicRoomsAndBookings([], [

            Room.Create("201", "DBL"),
        ]);
        
        var currentDate = new DateOnly(2024, 01, 01);
        int daysCountAheadOfSearch = 0; 
        int daysCount = 3; 

        var availability = hotel.SearchAvailability(RoomType.Double, daysCountAheadOfSearch, daysCount, () => currentDate);
        
        availability.Should().BeEquivalentTo(new List<(DateRange DateRange, int Rooms)>
        {
            (DateRange.FromDayNumber([738885,738886,738887]), 1),
        });
    }
    
    [Fact]
    public void SearchAvailability_One_Day_Booked_One_Day_Free_One_Day_Booked()
    {
        var bookings = new[]
        {
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 01))),
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 03), new DateOnly(2024, 01, 03))),
        };

        
        var hotel = HotelMother.DynamicRoomsAndBookings(bookings, [

            Room.Create("201", "DBL"),
        ]);
        
        var currentDate = new DateOnly(2024, 01, 01);
        int daysCountAheadOfSearch = 0; 
        int daysCount = 3; 

        var availability = hotel.SearchAvailability(RoomType.Double, daysCountAheadOfSearch, daysCount, () => currentDate);
        
        availability.Should().BeEquivalentTo(new List<(DateRange DateRange, int Rooms)>
        {
            (DateRange.FromDayNumber([738886]), 1),
        });
    }
    
    [Fact]
    public void SearchAvailability_One_Day_Free_One_Day_Booked_One_Day_Free()
    {
        var bookings = new[]
        {
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 02), new DateOnly(2024, 01, 02))),
        };
        
        var hotel = HotelMother.DynamicRoomsAndBookings(bookings, [

            Room.Create("201", "DBL"),
        ]);
        
        var currentDate = new DateOnly(2024, 01, 01);
        int daysCountAheadOfSearch = 0; 
        int daysCount = 3; 

        var availability = hotel.SearchAvailability(RoomType.Double, daysCountAheadOfSearch, daysCount, () => currentDate);
        
        availability.Should().BeEquivalentTo(new List<(DateRange DateRange, int Rooms)>
        {
            (DateRange.FromDayNumber([738885]), 1),
            (DateRange.FromDayNumber([738887]), 1),
        });
    }
    
    [Fact]
    public void SearchAvailability_One_Day_Last_Day_Booked()
    {
        var bookings = new[]
        {
            Booking.DBL(DateRange.Create(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 01))),
        };
        
        var hotel = HotelMother.DynamicRoomsAndBookings(bookings, [

            Room.Create("201", "DBL"),
        ]);
        
        var currentDate = new DateOnly(2024, 01, 01);
        int daysCountAheadOfSearch = 0; 
        int daysCount = 1; 

        var availability = hotel.SearchAvailability(RoomType.Double, daysCountAheadOfSearch, daysCount, () => currentDate);
        availability.Should().BeEmpty();
    }
    
    [Fact]
    public void SearchAvailability_JSON_Case()
    {
        var bookings = new[]
        {
            Booking.DBL(DateRange.Create(new DateOnly(2025, 11, 28), new DateOnly(2025, 12, 11))),
            Booking.DBL(DateRange.Create(new DateOnly(2025, 11, 28), new DateOnly(2025, 12, 11))),
        };
        
        var hotel = HotelMother.DynamicRoomsAndBookings(bookings, [

            Room.Create("201", "DBL"),
            Room.Create("201", "DBL"),
        ]);
        
        var currentDate = new DateOnly(2024, 11, 28);
        int daysCountAheadOfSearch = 365; 
        int daysCount = 14; 

        var availability = hotel.SearchAvailability(RoomType.Double, daysCountAheadOfSearch, daysCount, () => currentDate);
        availability.Should().BeEmpty();
    }
}