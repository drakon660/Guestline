using FluentAssertions;
using Guestline.Core;

namespace Guestline.Tests;

public class DateRangeTests
{
    [Fact]
    public void Test_DateRange()
    {
        var dateRange = DateRange.From("20200101-20200102");
        dateRange.StartDate.Should().Be(new DateOnly(2020, 01,01));
        dateRange.EndDate.Should().Be(new DateOnly(2020, 01,02));
    }
}