using System;
using System.Threading.Tasks;
using AdminBot.Common.CallbackQueries;
using AdminBot.Common.Messages;
using AdminBot.UseCases.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AdminBot.UseCases.Infrastructure.Internal
{
    public class MessageFormatter : IMessageFormatter
    {
        private readonly ITelegramBotClient _client;

        public MessageFormatter(
            ITelegramBotClient client)
        {
            _client = client;
        }

        public Task FormatAndSend(
            IMessage message,
            long chatId)
        {
            switch (message)
            {
                case WarnPersonMessage warnPersonMessage:
                    return OnWarnPerson(warnPersonMessage, chatId);
                
                case BanPersonMessage banPersonMessage:
                    return OnBanPerson(banPersonMessage, chatId);
                
                case WelcomePersonMessage welcomePersonMessage:
                    return OnWelcomePerson(welcomePersonMessage, chatId);
                
                case ChatRulesHasBeenChangedMessage chatRulesHasBeenChange:
                    return OnChatRulesHasBeenChanged(chatRulesHasBeenChange, chatId);
                
                case DescriptionMessage description:
                    return OnDescription(description, chatId);
                
                default: return Task.CompletedTask;
            }
        }

        private Task OnDescription(
            DescriptionMessage descriptionMsg,
            long chatId)
        {
            return _client.SendTextMessageAsync(
                chatId: chatId,
                text: descriptionMsg.Description,
                parseMode: ParseMode.Markdown);
        }

        private async Task OnChatRulesHasBeenChanged(
            ChatRulesHasBeenChangedMessage chatRulesHasBeenChange,
            long chatId)
        {
            await _client.SendTextMessageAsync(
                chatId: chatId,
                text: $"Правила чата были изменены! ");
        }

        private async Task OnWelcomePerson(
            WelcomePersonMessage welcomePersonMessage,
            long chatId)
        {
            var callbackQuery = CallbackQueryEnvelope.FromAcceptChatRules(
                acceptChatRulesCallbackQuery: new AcceptChatRulesCallbackQuery(
                    userId: welcomePersonMessage.UserId));

            var callbackData = JsonConvert.SerializeObject(callbackQuery);
            
            var button = new InlineKeyboardButton("Принимаю");
            button.CallbackData = callbackData;
            var keyboard = new InlineKeyboardMarkup(button);
            var agreement = welcomePersonMessage.Agreement;
            var mention = CreateMention(
                mention: welcomePersonMessage.UserName,
                userId: welcomePersonMessage.UserId);
            var text = $"{mention} чтобы писать сообщения нужно принять правила группы!\n" + agreement;

            await _client.SendTextMessageAsync(
                chatId:chatId,
                text: text,
                parseMode: ParseMode.Markdown,
                replyMarkup: keyboard);
        }

        private async Task OnBanPerson(
            BanPersonMessage banPersonMessage,
            long chatId)
        {
            var moscowTime = banPersonMessage.ExpireAt + TimeSpan.FromHours(9);
            
            await _client.SendTextMessageAsync(
                chatId: chatId,
                text: $"{CreateMention(banPersonMessage.UserName, banPersonMessage.UserId)} забанен до {moscowTime:g}! ",
                replyToMessageId: banPersonMessage.BlameMessageId,
                parseMode: ParseMode.Markdown);
        }

        private async Task OnWarnPerson(
            WarnPersonMessage warnPersonMessage,
            long chatId)
        {
            await _client.SendTextMessageAsync(
                chatId: chatId,
                text: $"Предупреждение {warnPersonMessage.Warns}! " +
                      $"Получите больше {warnPersonMessage.WarnsLimit} и будете забанены!",
                replyToMessageId: warnPersonMessage.BlameMessageId,
                parseMode: ParseMode.Markdown);
        }

        private static string CreateMention(
            string mention,
            long userId)
        {
            return $"[{mention}](tg://user?{userId})";
        }
    }
}