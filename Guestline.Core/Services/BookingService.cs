using System.Text;

namespace Guestline.Core.Services;

public class BookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IDateProvider _dateProvider;

    public BookingService(IBookingRepository bookingRepository, IDateProvider dateProvider)
    {
        _bookingRepository = bookingRepository;
        _dateProvider = dateProvider;
    }
    
    public async Task<string> CheckAvailability(string hotelId, string dateOrDateRange, string roomType)
    {
        Hotel hotel = await _bookingRepository.GetHotelById(hotelId);

        var dateRange = DateRange.From(dateOrDateRange);

        var availability = hotel.CheckAvailability(roomType, dateRange);
        
        return $"({dateRange.StartDate.ToString(Utility.DateFormat)}-{dateRange.EndDate.ToString(Utility.DateFormat)}), {availability}"; 
    }

    public async Task<string> Search(string hotelId, int daysAheadToSearch, int daysCount, string roomType)
    {
        Hotel hotel = await _bookingRepository.GetHotelById(hotelId);
        var futureAvailabilities =
            hotel.SearchAvailability(roomType, daysAheadToSearch, daysCount, () => _dateProvider.GetCurrentDate());

        var resultsBuilder = new StringBuilder();
        foreach (var futureAvailability in futureAvailabilities)
        {
            resultsBuilder.AppendFormat("({0}-{1}), {2}",
                futureAvailability.DateRange.StartDate.ToString(Utility.DateFormat),
                futureAvailability.DateRange.EndDate.ToString(Utility.DateFormat), futureAvailability.Rooms);
            resultsBuilder.Append(',');
        }

        if(resultsBuilder.Length > 0)
            resultsBuilder.Length -= 1;
        
        return resultsBuilder.ToString();
    }
}