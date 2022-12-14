using System;
using System.Threading.Tasks;

namespace AdminBot.Common.Queries
{
    public class RegisterPersonQuery
    {
        public RegisterPersonQuery(
            long userId,
            long chatId,
            string userName,
            string firstName,
            DateTime dateTime)
        {
            UserId = userId;
            ChatId = chatId;
            UserName = userName;
            DateTime = dateTime;
            FirstName = firstName;
        }
        
        public interface IHandler
        {
            Task<Person> HandleAsync(RegisterPersonQuery query);
        }
        
        public long UserId { get; }
        
        public long ChatId { get; }
        
        public string UserName { get; }
        
        public string FirstName { get; }
        
        public DateTime DateTime { get; }
    }
}