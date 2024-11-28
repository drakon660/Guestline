namespace Guestline.Core;

public sealed class FileBookingRepository : IBookingRepository
{
    private readonly FileDatabase _fileDatabase;

    public FileBookingRepository(FileDatabase fileDatabase)
    {
        _fileDatabase = fileDatabase;
    }


    public async Task<Hotel> GetHotelById(string id)
    {
        return (await _fileDatabase.GetHotels()).SingleOrDefault(x => x.Id == id);
    }
}