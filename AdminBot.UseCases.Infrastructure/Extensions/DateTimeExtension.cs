using System;

namespace AdminBot.UseCases.Infrastructure.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime RoundToSeconds(this DateTime datetime)
        {
            return new DateTime(
                datetime.Year,
                datetime.Month,
                datetime.Day,
                datetime.Hour,
                datetime.Minute,
                datetime.Second);
        }
    }
}