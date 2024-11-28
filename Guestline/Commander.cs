using System.Text.RegularExpressions;

namespace Guestline;

public record Command
{
    public string HotelId {get; init; }
    public string CommandName { get; init; }    // "Availability" or "Search"
    public string DateOrRange { get; init; }   // Date ("20240901") or range ("20240901-20240903")
    public int DaysCount { get; init; }       // Number of days (only for Search command)
    public string RoomType { get; init; }      // "SGL" or "DBL"
}

public class Commander
{
    public static Command ParseCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return null;
        
        var availabilitySingleDatePattern = @"^Availability\((\w+),\s*(\d{8}),\s*(SGL|DBL)\)$";
        var availabilityDateRangePattern = @"^Availability\((\w+),\s*(\d{8}-\d{8}),\s*(SGL|DBL)\)$";
        var searchPattern = @"^Search\((\w+),\s*(\d+),\s*(SGL|DBL)\)$";

        if (Regex.IsMatch(command, availabilitySingleDatePattern, RegexOptions.IgnoreCase))
        {
            var match = Regex.Match(command, availabilitySingleDatePattern);
            return new Command
            {
                CommandName = "Availability",
                HotelId = match.Groups[1].Value,
                DateOrRange = match.Groups[2].Value,
                RoomType = match.Groups[3].Value
            };
        }

        if (Regex.IsMatch(command, availabilityDateRangePattern, RegexOptions.IgnoreCase))
        {
            var match = Regex.Match(command, availabilityDateRangePattern);
            return new Command
            {
                CommandName = "Availability",
                HotelId = match.Groups[1].Value,
                DateOrRange = match.Groups[2].Value,
                RoomType = match.Groups[3].Value
            };
        }

        if (Regex.IsMatch(command, searchPattern, RegexOptions.IgnoreCase))
        {
            var match = Regex.Match(command, searchPattern);
            return new Command
            {
                CommandName = "Search",
                HotelId = match.Groups[1].Value,
                DaysCount = int.Parse(match.Groups[2].Value),
                RoomType = match.Groups[3].Value
            };
        }


        return null;
    }
    
    public static string GetArgumentValue(string[] args, string argumentName)
    {
        int index = Array.IndexOf(args, argumentName);
    
        if (index != -1 && index + 1 < args.Length)
        {
            return args[index + 1];
        }

        return null;
    }
}