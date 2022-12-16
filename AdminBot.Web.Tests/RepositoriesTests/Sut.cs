using AdminBot.Common;
using AdminBot.UseCases.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AdminBot.Web.Tests.RepositoriesTests;

public class Sut
{
    private readonly string _connectionString;
    private readonly IPersonsRepository _personsRepository;
    private readonly IChatSettingsRepository _chatSettingsRepository;
    private readonly IBanRepository _banRepository;

    public Sut(
        string connectionString,
        IPersonsRepository personsRepository,
        IChatSettingsRepository chatSettingsRepository,
        IBanRepository banRepository)
    {
        _personsRepository = personsRepository;
        _connectionString = connectionString;
        _chatSettingsRepository = chatSettingsRepository;
        _banRepository = banRepository;
    }

    public Task<Person> AddPersonAsync(
        long userId,
        long chatId,
        string userName,
        string firstName,
        DateTime createdAt)
    {
        return _personsRepository.AddPersonAsync(
            userId: userId,
            username: userName,
            chatId: chatId,
            createdAt: createdAt);
    }

    public Task IncrementWarnsAsync(
        Person person,
        DateTime datetime)
    {
        return _personsRepository.IncrementWarnsAsync(
            person: person,
            dateTime: datetime);
    }

    public Task<Person> FindPersonAsync(
        long userId,
        long chatId)
    {
        return _personsRepository.GetPersonAsync(
            userId: userId,
            chatId: chatId);
    }

    public Task<ChatSettings> FindChatAgreementAsync(
        long telegramId)
    {
        return _chatSettingsRepository
            .FindAsync(
                chatId: telegramId);
    }

    public async Task<PersonRecord> FindPersonRecordAsync(
        long userId,
        long chatId)
    {
        await using var connection = new SqlConnection(_connectionString);
        
        var result = await connection
            .QueryFirstOrDefaultAsync<PersonRecord>(
                sql: $"select * from Persons where ChatId={chatId} and UserId={userId}");

        return result;
    }
    
    public async Task<ChatSettingsRecord> FindChatAgreementRecordAsync(
        long telegramId)
    {
        await using var connection = new SqlConnection(_connectionString);
        
        var result = await connection
            .QueryFirstOrDefaultAsync<ChatSettingsRecord>(
                sql: $"select * from ChatSettings where TelegramId={telegramId}");

        return result;
    }

    public async Task InsertPersonRecordAsync(Person person)
    {
        var sql = "INSERT INTO Persons (UserId, ChatId, UserName, Warns, CreatedAt, UpdatedAt)"
        + $"VALUES ( " +
        $"{person.UserId}, " +
        $"{person.ChatId}, " +
        $"'{person.Username}', " +
        $"{person.Warns}, " +
        $"'{person.CreatedAt:s}', " +
        $"'{person.UpdatedAt:s}')";

        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql);
    }
    
    public async Task InsertChatSettingsRecordAsync(
        ChatSettings chatSettings)
    {
        var sql = "INSERT INTO ChatSettings (TelegramId, Agreement, WarnsLimit, CreatedAt, BanTtlTicks)" +
                  $"values (" +
                  $"{chatSettings.TelegramId}, " +
                  $"'{chatSettings.Agreement}', " +
                  $"{chatSettings.WarnsLimit}, " +
                  $"'{chatSettings.CreatedAt:s}', " +
                  $"{chatSettings.BanTtl.TotalSeconds})";

        await using var connection = new SqlConnection(_connectionString);
        
        await connection.ExecuteAsync(sql);
    }

    public async Task SaveChatSettingsAgreementAsync(
        long telegramChatId,
        string agreement,
        int warnsLimit,
        TimeSpan banTtl,
        DateTime dateTime)
    {
        await _chatSettingsRepository.SaveAgreementAsync(
            telegramId: telegramChatId,
            agreement: agreement,
            dateTime: dateTime,
            warnsLimit: warnsLimit,
            banTtl: banTtl);
    }

    private static string ToSqlFormat(DateTime dateTime)
    {
        return dateTime.ToString("s");
    }

    public async Task SaveBan(Ban ban)
    {
        await _banRepository.AddAsync(
            person: ban.Person,
            requestedAt: ban.CreatedAt,
            expireAt: ban.ExpireAt);
    }

    public async Task<BanRecord> SearchBanRecordByPersonId(int personId)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var result = await connection
            .QueryFirstOrDefaultAsync<BanRecord>(
                sql: $"select * from Bans where PersonId={personId}");

        return result;
    }

    public async Task SaveBanRecordAsync(Ban ban)
    {
        await using var connection = new SqlConnection(_connectionString);
        
        await connection
            .ExecuteAsync(
                sql: "INSERT INTO Bans (PersonId, CreatedAt, ExpireAt) "
                     + $"Values (@PersonId, @CreatedAt, @ExpireAt)",
                param: new
                {
                    PersonId = ban.Person.Id,
                    CreatedAt = ban.CreatedAt,
                    ExpireAt = ban.ExpireAt,
                });
    }

    public Task<Ban> FindBanForPersonAsync(Person person)
    {
        return  _banRepository.FindForPerson(person);
    }
}