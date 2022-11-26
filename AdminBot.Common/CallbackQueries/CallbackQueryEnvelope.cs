using System;

namespace AdminBot.Common.CallbackQueries;

public class CallbackQueryEnvelope
{
    public CallbackQueryEnvelope()
    {
    }
    
    public CallbackQueryEnvelope(AcceptChatRulesCallbackQuery acceptChatRulesCallbackQuery)
    {
        AcceptChatRulesCallbackQuery = acceptChatRulesCallbackQuery;
    }

    public AcceptChatRulesCallbackQuery? AcceptChatRulesCallbackQuery { get; set; }

    public static CallbackQueryEnvelope FromAcceptChatRules(
        AcceptChatRulesCallbackQuery acceptChatRulesCallbackQuery)
    {
        return new CallbackQueryEnvelope(acceptChatRulesCallbackQuery);
    }

    public T? Match<T>(
        Func<AcceptChatRulesCallbackQuery, T>? onAcceptChatRules = default,
        Func<T>? onDefault = default)
    {
        if (onAcceptChatRules != null && AcceptChatRulesCallbackQuery != null)
        {
            return onAcceptChatRules(this.AcceptChatRulesCallbackQuery);
        }

        if (onDefault != null)
        {
            return onDefault();
        }
        
        return default(T);
    }
}