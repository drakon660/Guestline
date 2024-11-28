using System.Text.Json.Serialization;

namespace Guestline.Core.Dtos;

public record HotelDto
{
    public string Id { get; init; }
    public string Name { get; init; }
    public IEnumerable<RoomTypeDto> RoomTypes { get; init; }
    public IEnumerable<RoomDto> Rooms { get; init; }
}