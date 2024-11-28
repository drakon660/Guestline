using CSharpFunctionalExtensions;

namespace Guestline.Core;

public class Booking : Entity<int>
{
    public RoomType RoomType { get; protected set; }
    
    public DateRange DateRange { get; protected set; }
    
    public string RoomRate { get; protected set; }
    
    private Booking(RoomType roomType, string roomRate, DateRange dateRange)
    {
        RoomType = roomType;
        RoomRate = roomRate;
        DateRange = dateRange;
    }
    
    public static Booking Create(string roomType, string roomRate, DateRange dateRange)=>new ((RoomType)roomType, roomRate, dateRange);
    public static Booking SGL(DateRange dateRange) => new(RoomType.Single, RoomRates.Standard, dateRange);
    public static Booking DBL(DateRange dateRange) => new(RoomType.Double, RoomRates.Prepaid, dateRange);
}