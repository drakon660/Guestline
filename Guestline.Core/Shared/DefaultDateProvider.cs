namespace Guestline.Core;

public class DefaultDateProvider : IDateProvider
{
    public DateOnly GetCurrentDate()
    {
        return DateOnly.FromDateTime(DateTime.Now);
    }
}