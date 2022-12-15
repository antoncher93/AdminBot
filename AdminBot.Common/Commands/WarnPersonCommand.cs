using System;
using System.Threading.Tasks;

namespace AdminBot.Common.Commands
{
    public class WarnPersonCommand
    {
        public WarnPersonCommand(
            Person person,
            int? messageId,
            DateTime requestTime,
            int warnsLimit)
        {
            Person = person;
            MessageId = messageId;
            RequestTime = requestTime;
            WarnsLimit = warnsLimit;
        }
        
        public interface IHandler
        {
            Task HandleAsync(
                WarnPersonCommand command);
        }

        public Person Person { get; }
        public int? MessageId { get; }
        public DateTime RequestTime { get; }
        public int WarnsLimit { get; }
    }
}