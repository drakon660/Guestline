using CSharpFunctionalExtensions;

namespace Guestline.Core;

public class RoomType : ValueObject
{
    private static readonly HashSet<string> _roomTypes = ["SGL","DBL"];
    
    public string Value { get; }

    private RoomType(string value)
    {
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }


    public static RoomType Create(string value)
    {
        if(!_roomTypes.Contains(value))
            throw new ArgumentException($"Value {value} is not a valid room type");

        return new(value);
    }
    

    public static RoomType Single => new ("SGL");
    public static RoomType Double => new ("DBL");
    
    public static implicit operator string(RoomType roomType) => roomType.Value;

    public static explicit operator RoomType(string roomType) => new (roomType);
    
    public static bool operator ==(RoomType left, RoomType right) => left?.Value == right?.Value;

    public static bool operator !=(RoomType left, RoomType right) => !(left == right);
}