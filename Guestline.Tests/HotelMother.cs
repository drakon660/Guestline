using Guestline.Core;

namespace Guestline.Tests;

public static class HotelMother
{
    public static Hotel TwoSGLTwoDBL(Booking[] bookings)
    {
        Room sgl101 = Room.Create("101", "SGL");
        Room sgl102 = Room.Create("102", "SGL");
        Room dbl201 = Room.Create("201", "DBL");
        Room dbl202 = Room.Create("202", "DBL");

        return Hotel.Create("H1", "Verona", [sgl101, sgl102, dbl201, dbl202], bookings);
    }

    public static Hotel DynamicRoomsAndBookings(Booking[] bookings, List<Room> rooms)
    {
        return Hotel.Create("H1", "Verona", rooms, bookings);
    }
}