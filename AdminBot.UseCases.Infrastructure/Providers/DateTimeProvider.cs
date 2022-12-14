using System;
using AdminBot.UseCases.Providers;

namespace AdminBot.UseCases.Infrastructure.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }
    }
}