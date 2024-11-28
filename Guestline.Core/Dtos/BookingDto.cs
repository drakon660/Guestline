namespace Guestline.Core.Dtos;

public record BookingDto
{
    public string HotelId { get; init; }
    public string Arrival { get; init; }
    public string Departure { get; init; }
    public string RoomType { get; init; }
    public string RoomRate { get; init; }
}