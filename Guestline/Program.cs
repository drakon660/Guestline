using Guestline;
using Guestline.Core;
using Guestline.Core.Services;

var hotelsFilePath = Commander.GetArgumentValue(args, "--hotels");
var bookingsFilePath = Commander.GetArgumentValue(args, "--bookings");

if (hotelsFilePath == null)
{
    Console.WriteLine("Error: The '--hotels' argument is required.");
    return;
}

if (bookingsFilePath == null)
{
    Console.WriteLine("Error: The '--bookings' argument is required.");
    return;
}

if (!File.Exists(hotelsFilePath))
{
    Console.WriteLine($"Error: The file '{hotelsFilePath}' does not exist.");
    return;
}

if (!File.Exists(bookingsFilePath))
{
    Console.WriteLine($"Error: The file '{bookingsFilePath}' does not exist.");
    return;
}

int daysCount = 14;

IBookingRepository fileBookingRepository = new FileBookingRepository(new FileDatabase(bookingsFilePath, hotelsFilePath));
IDateProvider localDateProvider = new DefaultDateProvider();

var bookingService = new BookingService(fileBookingRepository, localDateProvider);

while (true)
{
    var inputLine = Console.ReadLine();
    var command = Commander.ParseCommand(inputLine);
    if (command == null)
    {
        Console.WriteLine("Error: Invalid command format. Please provide a valid command.");
        Console.WriteLine("Valid formats are:");
        Console.WriteLine("  Availability(H1, 20240901, SGL)");
        Console.WriteLine("  Availability(H1, 20240901-20240903, DBL)");
        Console.WriteLine("  Search(H1, 365, SGL)");
        continue;
    }

    switch (command.CommandName)
    {
        case "Availability":
            var availability = await bookingService.CheckAvailability(command.HotelId, command.DateOrRange, command.RoomType);
            Console.WriteLine(availability);
            break;
        case "Search":
            var search = await bookingService.Search(command.HotelId, command.DaysCount, daysCount, command.RoomType);
            Console.WriteLine(search);
            break;
        default:
            Console.WriteLine("Error: Invalid command format. Please provide a valid command.");
            break;
    }
}
