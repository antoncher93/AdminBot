create table ChatSettings
(
    Id          int identity primary key,
    TelegramId  bigint   not null,
    Agreement   nvarchar(max),
    WarnsLimit  int      not null,
    CreatedAt   DATETIME not null,
    BanTtlTicks bigint   not null
)
go

create table Persons
(
    Id        int IDENTITY (1,1) PRIMARY KEY not null,
    UserId    bigint                         not null,
    ChatId    bigint                         not null,
    UserName  nvarchar(50)                   not null,
    FirstName nvarchar(50)                   not null,
    Warns     int                            not null,
    CreatedAt DATETIME                       not null,
    UpdatedAt DATETIME                       not null,
)
go

CREATE TABLE Bans
(
    Id        int IDENTITY (1,1) PRIMARY KEY not null,
    PersonId  int                            not null,
    CreatedAt DATETIME                       not null,
    ExpireAt  DATETIME                       not null,
)
go