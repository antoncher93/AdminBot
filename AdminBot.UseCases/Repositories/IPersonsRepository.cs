using System;
using System.Threading;
using System.Threading.Tasks;
using AdminBot.Common;

namespace AdminBot.UseCases.Repositories
{
    public interface IPersonsRepository
    {
        Task<Person> GetPersonAsync(
            long userId,
            long chatId,
            CancellationToken token = default);

        Task<Person> AddPersonAsync(
            long userId,
            long chatId,
            string username,
            DateTime createdAt);
        
        Task<int> IncrementWarnsAsync(
            Person person,
            DateTime dateTime);

        Task ResetWarns(int personId,
            DateTime dateTime);
    }
}