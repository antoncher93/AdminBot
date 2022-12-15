using AdminBot.Common;
using FluentAssertions;
using Xunit;

namespace AdminBot.Web.Tests.RepositoriesTests;

public class ChatSettingsRepositoriesTest
{
    [Fact]
    public async Task FindsChatAgreement()
    {
        var dateTime = DateTime.Today;
        var chatSettings = ObjectsGen.CreateRandomChatSettings(
            createdAt: dateTime);

        var sut = SutFactory.Create();

        await sut.InsertChatSettingsRecordAsync(chatSettings);

        var actual = await sut.FindChatAgreementAsync(
            telegramId: chatSettings.TelegramId);

        actual
            .Should()
            .BeEquivalentTo(chatSettings);
    }

    [Fact]
    public async Task SavesChatSettingsAgreement()
    {
        var sut = SutFactory.Create();
        var dateTime = DateTime.Today;
        var chatSettings = ObjectsGen.CreateRandomChatSettings(
            createdAt: dateTime);

        await sut.SaveChatSettingsAgreementAsync(
            telegramChatId: chatSettings.TelegramId,
            agreement: chatSettings.Agreement,
            warnsLimit: chatSettings.WarnsLimit,
            banTtl: chatSettings.BanTtl,
            dateTime: dateTime);

        var record = await sut.FindChatAgreementRecordAsync(
            telegramId: chatSettings.TelegramId);

        var actual = MapChatAgreementFromRecord(record);

        actual.Should()
            .BeEquivalentTo(
                chatSettings);
    }

    private static ChatSettings MapChatAgreementFromRecord(
        ChatSettingsRecord record)
    {
        return new ChatSettings(
            telegramId: record.TelegramId,
            agreement: record.Agreement,
            warnsLimit: record.WarnsLimit,
            createdAt: record.CreatedAt,
            banTtl: TimeSpan.FromSeconds(record.BanTtlTicks));
    }
}