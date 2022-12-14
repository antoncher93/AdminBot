using AdminBot.UseCases.Providers;

namespace AdminBot.Web.Tests.Fakes;

public class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime GetDateTimeNow()
    {
        return DateTime.Parse("2022-12-14T17:46:00");
    }
}