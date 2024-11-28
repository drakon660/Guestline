using System.Text.Json;
using Guestline.Core.Dtos;

namespace Guestline.Core;

public class FileDatabase
{
    private readonly string _bookingFilePath;
    private readonly string _hotelFilePath;

    public FileDatabase(string bookingFilePath, string hotelFilePath)
    {
        _bookingFilePath = bookingFilePath;
        _hotelFilePath = hotelFilePath;
    }

    public async Task<IReadOnlyList<Hotel>> GetHotels()
    {
        var hotelFileContent = await File.ReadAllTextAsync(_hotelFilePath);
        var bookingFileContent = await File.ReadAllTextAsync(_bookingFilePath);
        
        var hotelsData = JsonSerializer.Deserialize<List<HotelDto>>(hotelFileContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var bookingsData = JsonSerializer.Deserialize<List<BookingDto>>(bookingFileContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        List<Hotel> hotels = new List<Hotel>();

        foreach (var hotelData in hotelsData)
        {
            var bookings = bookingsData.Where(x => x.HotelId == hotelData.Id)
                .Select(x => Booking.Create(x.RoomType, x.RoomRate, DateRange.Create(x.Arrival, x.Departure))).ToList();

            var hotel = Hotel.Create(hotelData.Id, hotelData.Name,
                hotelData.Rooms.Select(x => Room.Create(x.RoomId, x.RoomType)).ToList(), bookings);
            
            hotels.Add(hotel);
        }

        return hotels;
    }
}