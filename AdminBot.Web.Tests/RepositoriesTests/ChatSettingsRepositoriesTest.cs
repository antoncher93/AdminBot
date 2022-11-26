using AdminBot.Common;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace AdminBot.Web.Tests.RepositoriesTests;

public class ChatSettingsRepositoriesTest
{
    [Fact]
    public async Task FindsChatAgreement()
    {
        var chatSettings = ObjectsGen.CreateRandomChatSettings();

        var sut = SutFactory.Create();

        await sut.InsertChatSettingsRecordAsync(chatSettings);

        var actual = await sut.FindChatAgreementAsync(chatSettings.TelegramId);

        actual
            .Should()
            .BeEquivalentTo(
                chatSettings, 
                options => options
                    .Using<DateTime>(ctx
                        => ctx.Subject
                            .Should()
                            .BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task SavesChatSettingsAgreement()
    {
        var chatSettings = ObjectsGen.CreateRandomChatSettings();
        var dateTime = DateTime.Now;
        var sut = SutFactory.Create();

        await sut.SaveChatSettingsAgreementAsync(
            telegramChatId: chatSettings.TelegramId,
            agreement: chatSettings.Agreement,
            dateTime: dateTime);

        var record = await sut.FindChatAgreementRecordAsync(
            telegramId: chatSettings.TelegramId);

        var actual = MapChatAgreementFromRecord(record);

        actual.Agreement
            .Should()
            .BeEquivalentTo(
                chatSettings.Agreement);
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