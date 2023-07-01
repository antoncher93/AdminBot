using AdminBot.UseCases.Infrastructure.Extensions;
using AdminBot.UseCases.Providers;

namespace AdminBot.Web.Tests.Fakes;

public class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime GetUtcNow()
    {
        return DateTime.UtcNow.RoundToSeconds();
    }
}