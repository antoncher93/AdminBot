using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Clients;
using AdminBot.UseCases.Repositories;

namespace AdminBot.UseCases.CommandHandlers
{
    public class SaveChatAgreementCommandHandler : SaveChatSettingsCommand.IHandler
    {
        private readonly IChatSettingsRepository _chatSettings;
        private readonly IBotClient _botClient;

        public SaveChatAgreementCommandHandler(
            IChatSettingsRepository chatSettings,
            IBotClient botClient)
        {
            _chatSettings = chatSettings;
            _botClient = botClient;
        }

        public async Task HandleAsync(SaveChatSettingsCommand command)
        {
            await _chatSettings
                .SaveAgreementAsync(
                    telegramId: command.TelegramId,
                    agreement: command.Agreement,
                    dateTime: command.ExecutedAt,
                    warnsLimit: command.WarnsLimit,
                    banTtl: command.BanTtl);

            await _botClient
                .SendMessageAsync(
                    message: new ChatRulesHasBeenChangedMessage(
                        agreement: command.Agreement),
                    chatId: command.TelegramId);
        }
    }
}