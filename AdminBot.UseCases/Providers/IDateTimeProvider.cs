using System;

namespace AdminBot.UseCases.Providers
{
    public interface IDateTimeProvider
    {
        DateTime GetUtcNow();
    }
}