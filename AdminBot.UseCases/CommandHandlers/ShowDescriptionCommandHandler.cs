using System.Text;
using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Clients;

namespace AdminBot.UseCases.CommandHandlers
{
    public class ShowDescriptionCommandHandler : ShowDescriptionCommand.IHandler
    {
        private readonly string _descriptionFilePath;
        private readonly IBotClient _botClient;

        public ShowDescriptionCommandHandler(
            IBotClient botClient,
            string descriptionFilePath)
        {
            _botClient = botClient;
            _descriptionFilePath = descriptionFilePath;
        }

        public async Task HandleAsync(ShowDescriptionCommand command)
        {
            var description = System.IO.File.ReadAllText(
                path: _descriptionFilePath,
                encoding: Encoding.UTF8);

            await _botClient
                .SendMessageAsync(
                    chatId: command.ChatId,
                    message: new DescriptionMessage(
                        description: description));
        }
    }
}