using System.Text;
using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Adapters;

namespace AdminBot.UseCases.CommandHandlers
{
    public class ShowDescriptionCommandHandler : StartBotCommandCommand.IHandler
    {
        private readonly string _descriptionFilePath;
        private readonly IBotClientAdapter _botClientAdapter;

        public ShowDescriptionCommandHandler(
            IBotClientAdapter botClientAdapter,
            string descriptionFilePath)
        {
            _botClientAdapter = botClientAdapter;
            _descriptionFilePath = descriptionFilePath;
        }

        public async Task HandleAsync(StartBotCommandCommand commandCommand)
        {
            var description = System.IO.File.ReadAllText(
                path: _descriptionFilePath,
                encoding: Encoding.UTF8);

            await _botClientAdapter
                .SendMessageAsync(
                    chatId: commandCommand.ChatId,
                    message: new DescriptionMessage(
                        description: description));
        }
    }
}