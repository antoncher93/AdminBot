using AdminBot.UseCases.Clients;
using AdminBot.UseCases.Infrastructure.Internal;
using Telegram.Bot;

namespace AdminBot.UseCases.Infrastructure.Clients
{
    public static class BotClientFactory
    {
        public static IBotClient Create(string token)
        {
            var telegramBotClient = new TelegramBotClient(token);
            return new BotClient(
                client: telegramBotClient,
                messageFormatter: new MessageFormatter(telegramBotClient));
        }
    }
}