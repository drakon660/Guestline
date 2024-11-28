using System.Text.RegularExpressions;

namespace Guestline.Core;

public sealed class DateRange
{
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; private set; }
    
    public const string Pattern = @"^(?<StartDate>\d{8})(-(?<EndDate>\d{8}))?$";

    private DateRange(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    public static DateRange FromDayNumber(IEnumerable<int> dayNumbers)
    {
        var startDate = DateOnly.FromDayNumber(dayNumbers.First());
        var endDate = DateOnly.FromDayNumber(dayNumbers.Last());

        return new(startDate, endDate);
    }
    
    public static DateRange From(string dateOrDateRange)
    {
        var match = Regex.Match(dateOrDateRange, Pattern);

        if (match.Success)
        {
            string startDate = match.Groups["StartDate"].Value;
            string endDate = match.Groups["EndDate"].Success ? match.Groups["EndDate"].Value : startDate;

            return Create(startDate, endDate);
        }

        return null;
    }

    public static DateRange FromDayNumber(int dayNumber)
    {
        var startDate = DateOnly.FromDayNumber(dayNumber);

        return new(startDate, startDate);
    }
    
    public static DateRange FromDayNumber(int[] dayNumbers)
    {
        var startDate = DateOnly.FromDayNumber(dayNumbers.First());
        var endDate = DateOnly.FromDayNumber(dayNumbers.Last());

        return new(startDate, endDate);
    }

    public static DateRange From(DateOnly startDate, int daysCount) => new(startDate, startDate.AddDays(daysCount));

    public static DateRange Create(DateOnly startDate, DateOnly endDate)
    {
        if(startDate > endDate)
            throw new ArgumentException("Start date cannot be greater than the end date");
        
        return new DateRange(startDate, endDate);
    }

    public static DateRange Create(string startDate, string endDate)
    {
        if (!DateOnly.TryParseExact(startDate,Utility.DateFormat, out var parsedStartDate))
        {
            throw new ArgumentException($"Invalid start date format: {startDate}", nameof(startDate));
        }

        if (!DateOnly.TryParseExact(endDate,Utility.DateFormat, out var parsedEndDate))
        {
            throw new ArgumentException($"Invalid end date format: {endDate}", nameof(endDate));
        }

        return Create(parsedStartDate, parsedEndDate);
    }

    public void ExtendByDays(int daysCount)
    {
        EndDate = EndDate.AddDays(daysCount);
    }

    public int[] DayNumbers
    {
        get
        {
            var days = new List<int>();
            var current = StartDate;
            while (current <= EndDate)
            {
                days.Add(current.DayNumber);
                current = current.AddDays(1);
            }

            return days.ToArray();
        }
    }
}