using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;

namespace Guestline.Core;

public class Hotel : Entity<string>
{
    public string Name { get; protected set; }
    public IReadOnlyList<Room> Rooms { get; protected set; }
    public IReadOnlyList<Booking> Bookings { get; protected set; }

    private Hotel(string name, IReadOnlyList<Room> rooms, IReadOnlyList<Booking> bookings)
    {
        Name = name;
        Rooms = rooms;
        Bookings = bookings;
    }
    
    private Hotel(string id, string name, IReadOnlyList<Room> rooms, IReadOnlyList<Booking> bookings) : base(id)
    {
        Name = name;
        Rooms = rooms;
        Bookings = bookings;
    }

    public static Hotel Create(string id, string name, IReadOnlyList<Room> rooms, IReadOnlyList<Booking> bookings) =>
        new(id, name, rooms, bookings);

    public int CheckAvailability(string roomType, DateRange dateRange)
    {
        var allRooms = Rooms.Where(x => x.RoomType == roomType).ToList();

        var overlappingBookings = Bookings
            .Where(x => x.RoomType == roomType && x.DateRange.Overlaps(dateRange))
            .ToList();

        if (!overlappingBookings.Any())
            return allRooms.Count;

        var roomUsage = CalculateRoomUsage(roomType, dateRange);

        int maxRoomsInUse = 0;

        foreach (var dayNumber in dateRange.DayNumbers)
        {
            if (roomUsage.TryGetValue(dayNumber, out int value) && value > maxRoomsInUse)
            {
                maxRoomsInUse = value;
            }
        }

        return Math.Max(0, allRooms.Count - maxRoomsInUse);
    }

    private IDictionary<int, int> CalculateRoomUsage(string roomType, DateRange dateRange)
    {
        var overlappingBookings = Bookings
            .Where(x => x.RoomType == roomType && x.DateRange.Overlaps(dateRange))
            .ToList();

        var roomUsage = new Dictionary<int, int>();
        foreach (var booking in overlappingBookings)
        {
            var dayNumbers = booking.DateRange.DayNumbers;

            foreach (var dayNumber in dayNumbers)
            {
                roomUsage.TryAdd(dayNumber, 0);
                roomUsage[dayNumber]++;
            }
        }

        return roomUsage;
    }
    
    public IReadOnlyList<(DateRange DateRange, int Rooms)> SearchAvailability(
       string roomType,
        int daysCountAheadOfSearch,
        int daysCount,
        Func<DateOnly> currentDate)
    {
        Guard.Against.Negative(daysCountAheadOfSearch, nameof(daysCountAheadOfSearch));
        Guard.Against.Negative(daysCount, nameof(daysCount));
        
        var dateRangeToSearch = Enumerable.Range(currentDate().DayNumber + daysCountAheadOfSearch, daysCount).ToList();
        var totalRoomsCount = GetTotalRomsCount(roomType);

        RoomsAvailability availability = RoomsAvailability.Empty();
        List<(DateRange DateRange, int Rooms)> roomsAvailabilities = new();
        
        var roomUsage = CalculateRoomUsage(roomType, DateRange.FromDayNumber(dateRangeToSearch));
        
        foreach (var dayNumber in dateRangeToSearch)
        {
            roomUsage.TryGetValue(dayNumber, out int value);

            var roomsLeft = totalRoomsCount - value;

            if (roomsLeft == totalRoomsCount || roomsLeft > 0) // Fully or partially available
            {
                HandleAvailability(dayNumber, dateRangeToSearch, ref availability, roomsAvailabilities, roomsLeft);
            }
            else // Fully booked
            {
                HandleFullBooking(ref availability, roomsAvailabilities);
            }
        }

        return roomsAvailabilities;
    }

    public int GetTotalRomsCount(string roomType) => Rooms.Count(x => x.RoomType == roomType);
    private void HandleAvailability(
        int dayNumber,
        List<int> dateRangeToSearch,
        ref RoomsAvailability availability,
        List<(DateRange DateRange, int Rooms)> roomsAvailabilities,
        int roomsLeft)
    {
        if (availability.IsEmpty)
        {
            availability.Set(DateRange.FromDayNumber(dayNumber), roomsLeft);
        }
        else if (availability.RoomCount != roomsLeft)
        {
            roomsAvailabilities.Add((availability.DateRange, availability.RoomCount));
            availability.Set(DateRange.FromDayNumber(dayNumber), roomsLeft);
        }
        else
        {
            availability.Extend(1);
        }
    
        if (dayNumber == dateRangeToSearch.Last())
        {
            roomsAvailabilities.Add((availability.DateRange, availability.RoomCount));
        }
    }
    
    private void HandleFullBooking(
        ref RoomsAvailability availability,
        List<(DateRange DateRange, int Rooms)> roomsAvailabilities)
    {
        if (!availability.IsEmpty)
        {
            roomsAvailabilities.Add((availability.DateRange, availability.RoomCount));
            availability = RoomsAvailability.Empty();
        }
    }
}