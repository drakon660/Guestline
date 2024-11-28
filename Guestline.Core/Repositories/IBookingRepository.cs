namespace Guestline.Core;

public interface IBookingRepository
{
    Task<Hotel> GetHotelById(string id);
}