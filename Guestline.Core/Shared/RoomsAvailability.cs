namespace Guestline.Core;

public class RoomsAvailability
{
    public DateRange DateRange { get; private set; }
    public bool IsEmpty => DateRange == null || RoomCount == 0;
    
    public void Extend(int dayNumber)
    {
        DateRange.ExtendByDays(dayNumber);
    }
    private RoomsAvailability() { } 

    public static RoomsAvailability Empty()
    {
        return new RoomsAvailability
        {
            DateRange = null,
            RoomCount = 0
        };
    }
    public void Set(DateRange dateRange, int roomCount)
    {
        DateRange = dateRange;
        RoomCount = roomCount;
    }

    public int RoomCount { get; private set; }
}