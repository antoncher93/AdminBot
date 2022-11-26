using System.Threading.Tasks;
using AdminBot.Common.Messages;

namespace AdminBot.UseCases.Infrastructure.Interfaces
{
    public interface IMessageFormatter
    {
        Task FormatAndSend(IMessage message, long chatId);
    }
}