using System;
using AdminBot.UseCases.Infrastructure.Extensions;
using AdminBot.UseCases.Providers;

namespace AdminBot.UseCases.Infrastructure.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}