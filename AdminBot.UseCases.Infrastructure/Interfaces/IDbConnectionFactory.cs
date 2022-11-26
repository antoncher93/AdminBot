using System.Data;

namespace AdminBot.UseCases.Infrastructure.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}