using System.Threading.Tasks;

namespace AdminBot.Common.Commands
{
    public class WelcomePersonCommand
    {
        public WelcomePersonCommand(
            Person person,
            string agreement)
        {
            Person = person;
            Agreement = agreement;
        }

        public interface IHandler
        {
            Task HandleAsync(WelcomePersonCommand command);
        }
        
        public Person Person { get; }
        public string Agreement { get; }
    }
}