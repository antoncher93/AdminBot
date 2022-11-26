INSERT INTO ChatSettings (TelegramId, Agreement, WarnsLimit, CreatedAt, BanTtlTicks)
Values (
           @TelegramId,
           @Agreement,
           @WarnsLimit,
           @CreatedAt,
           @BanTtlTicks)