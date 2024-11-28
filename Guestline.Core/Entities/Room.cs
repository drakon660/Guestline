using CSharpFunctionalExtensions;

namespace Guestline.Core;

public class Room : Entity<string>
{
    public Booking Booking { get; set; }
    
    public RoomType RoomType { get; set; }

    private Room(string id, string roomType) : base(id)
    {
        RoomType = RoomType.Create(roomType);
    }

    public static Room Create(string id, string roomType) => new (id, roomType);

    public static Room Single(string id) => new Room(id, RoomType.Single);
    public static Room Double(string id) => new Room(id, RoomType.Double);
}