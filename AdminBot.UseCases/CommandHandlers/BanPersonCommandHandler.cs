using System;
using System.Threading.Tasks;
using AdminBot.Common.Commands;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Clients;
using AdminBot.UseCases.Repositories;

namespace AdminBot.UseCases.CommandHandlers
{
    public class BanPersonCommandHandler : BanPersonCommand.IHandler
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly IBanRepository _banRepository;
        private readonly IChatSettingsRepository _chatSettingsRepository;
        private readonly IBotClient _botClient;
        private readonly TimeSpan _defaultBanTtl;

        public BanPersonCommandHandler(
            IPersonsRepository personsRepository,
            IBanRepository banRepository,
            IChatSettingsRepository chatSettingsRepository,
            IBotClient botClient,
            TimeSpan defaultBanTtl)
        {
            _personsRepository = personsRepository;
            _banRepository = banRepository;
            _defaultBanTtl = defaultBanTtl;
            _botClient = botClient;
            _chatSettingsRepository = chatSettingsRepository;
        }

        public async Task HandleAsync(BanPersonCommand command)
        {
            await _personsRepository.ResetWarns(
                personId: command.Person.Id,
                dateTime: command.RequestedAt)
                .ConfigureAwait(false);

            var chatSettings = await _chatSettingsRepository
                .FindAsync(
                    chatId: command.Person.ChatId)
                .ConfigureAwait(false);

            var banTtl = chatSettings?.BanTtl ?? _defaultBanTtl;

            var expireAt = command.RequestedAt + banTtl;

            await _banRepository.AddAsync(
                person: command.Person,
                requestedAt: command.RequestedAt,
                expireAt: expireAt)
                .ConfigureAwait(false);

            await _botClient.RestrictUserInChatAsync(
                userId: command.Person.UserId,
                chatId: command.Person.ChatId,
                untilDateTime: expireAt);

            var updatedPerson = await _personsRepository.GetPersonAsync(
                userId: command.Person.UserId,
                chatId: command.Person.ChatId);
            
            await _botClient
                .SendMessageAsync(
                    message: new BanPersonMessage(
                        blameMessageId: command.MessageId,
                        userId: updatedPerson.UserId,
                        userName: updatedPerson.Username,
                        expireAt: expireAt),
                    chatId: command.Person.ChatId);
        }
    }
}