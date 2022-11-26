UPDATE ChatSettings SET
                        Agreement = @Agreement,
                        BanTtlTicks = @BanTtlTicks
WHERE Id=@Id