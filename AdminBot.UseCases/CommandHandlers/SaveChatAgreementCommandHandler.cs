using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Adapters;
using AdminBot.UseCases.Repositories;

namespace AdminBot.UseCases.CommandHandlers
{
    public class SaveChatAgreementCommandHandler : SaveChatSettingsCommand.IHandler
    {
        private readonly IChatSettingsRepository _chatSettings;
        private readonly IBotClientAdapter _botClientAdapter;

        public SaveChatAgreementCommandHandler(
            IChatSettingsRepository chatSettings,
            IBotClientAdapter botClientAdapter)
        {
            _chatSettings = chatSettings;
            _botClientAdapter = botClientAdapter;
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

            await _botClientAdapter
                .SendMessageAsync(
                    message: new ChatRulesHasBeenChangedMessage(
                        agreement: command.Agreement),
                    chatId: command.TelegramId);
        }
    }
}