using Newtonsoft.Json;

namespace AdminBot.Common.CallbackQueries;

public class AcceptChatRulesCallbackQuery
{
    [JsonConstructor]
    public AcceptChatRulesCallbackQuery(
        long userId)
    {
        UserId = userId;
    }
    
    public long UserId { get; set; }
}