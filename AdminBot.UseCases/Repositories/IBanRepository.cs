using System;
using System.Threading.Tasks;
using AdminBot.Common;

namespace AdminBot.UseCases.Repositories
{
    public interface IBanRepository
    {
        Task<Ban> AddAsync(Person person,
            DateTime requestedAt,
            DateTime expireAt);
        
        Task<Ban> FindForPerson(Person person);
    }
}