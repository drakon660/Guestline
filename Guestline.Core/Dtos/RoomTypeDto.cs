namespace Guestline.Core.Dtos;

public record RoomTypeDto
{
    public string Code { get; init; }
    public string Description { get; init; }
    public IEnumerable<string> Amenities { get; init; }
    public IEnumerable<string> Features { get; init; }
}