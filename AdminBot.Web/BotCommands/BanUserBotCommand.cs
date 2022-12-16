﻿namespace AdminBot.Web.BotCommands;

public class BanUserBotCommand
{
    public BanUserBotCommand(
        long userId, 
        long chatId,
        int blameMessageId,
        string username,
        long senderId,
        DateTime executedAt)
    {
        UserId = userId;
        ChatId = chatId;
        BlameMessageId = blameMessageId;
        Username = username;
        ExecutedAt = executedAt;
        SenderId = senderId;
    }
    
    public interface IHandler
    {
        Task HandleAsync(
            BanUserBotCommand command);
    }
    
    public long UserId { get; }
    public string Username { get; }
    public long ChatId { get; }
    public int BlameMessageId { get; }
    public DateTime ExecutedAt { get; }
    public long SenderId { get; }
}