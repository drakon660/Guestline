using FluentAssertions;
using Guestline.Core;

namespace Guestline.Tests;

public class FileDatabaseTests
{
    [Fact]
    public async Task Check_If_Data_Files_Are_Being_Load_Correctly()
    {
        var fileDatabase = new FileDatabase("bookings.json", "hotels.json");
        var hotels = await fileDatabase.GetHotels(); 
        hotels.Should().BeEquivalentTo([
            Hotel.Create("H1", "Hotel California", [Room.Create("101", "SGL")],
                [
                    Booking.Create("DBL", "Prepaid", DateRange.Create("20240901", "20240903")),
                    Booking.Create("SGL", "Standard", DateRange.Create("20240902", "20240905")),
                ]
            )
        ]);
    }
}