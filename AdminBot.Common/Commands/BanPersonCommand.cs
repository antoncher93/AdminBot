using System;
using System.Threading.Tasks;

namespace AdminBot.Common.Commands
{
    public class BanPersonCommand
    {
        public BanPersonCommand(
            Person person,
            DateTime requestedAt,
            int messageId)
        {
            Person = person;
            RequestedAt = requestedAt;
            MessageId = messageId;
        }

        public interface IHandler
        {
            Task HandleAsync(BanPersonCommand command);
        }
        
        public Person Person { get; }
        public DateTime RequestedAt { get; }
        public int MessageId { get; }
    }
}