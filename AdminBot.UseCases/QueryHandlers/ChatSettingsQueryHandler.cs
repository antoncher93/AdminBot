﻿using System.Threading.Tasks;
using AdminBot.Common;
using AdminBot.Common.Queries;
using AdminBot.UseCases.Repositories;

namespace AdminBot.UseCases.QueryHandlers
{
    public class ChatSettingsQueryHandler : ChatSettingsQuery.IHandler
    {
        private readonly IChatSettingsRepository _chatSettings;

        public ChatSettingsQueryHandler(IChatSettingsRepository chatSettings)
        {
            _chatSettings = chatSettings;
        }

        public async Task<ChatSettings> HandleAsync(ChatSettingsQuery query)
        {
            return await _chatSettings.FindAsync(
                    chatId: query.ChatId)
                .ConfigureAwait(false);
        }
    }
}