INSERT INTO Persons (UserId, ChatId, UserName, Warns, CreatedAt, UpdatedAt)
VALUES (@UserId, @ChatId, @UserName, @Warns, @CreatedAt, @UpdatedAt)
SELECT Id FROM Persons
WHERE ChatId=@ChatId AND UserId=@UserId