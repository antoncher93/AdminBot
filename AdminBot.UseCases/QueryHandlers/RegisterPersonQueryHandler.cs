using System.Threading.Tasks;
using AdminBot.Common;
using AdminBot.Common.Queries;
using AdminBot.UseCases.Repositories;

namespace AdminBot.UseCases.QueryHandlers
{
    public class RegisterPersonQueryHandler : RegisterPersonQuery.IHandler
    {
        private readonly IPersonsRepository _persons;

        public RegisterPersonQueryHandler(IPersonsRepository persons)
        {
            _persons = persons;
        }

        public async Task<Person> HandleAsync(RegisterPersonQuery query)
        {
            return await _persons.GetPersonAsync(
                           userId: query.UserId,
                           chatId: query.ChatId)
                       .ConfigureAwait(false)
                   ?? await _persons.AddPersonAsync(
                           userId: query.UserId,
                           chatId: query.ChatId,
                           username: query.UserName,
                           createdAt: query.DateTime)
                       .ConfigureAwait(false);
        }
    }
}