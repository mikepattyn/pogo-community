-- Bot-specific tables for SQL Server
-- These tables store data that was previously in Google Cloud Datastore

USE pogo;
GO

-- Create bot_raids table (replaces Datastore Raids collection)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bot_raids]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[bot_raids] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Guid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [DateEnd] DATETIME2 NULL,
        [DateStart] DATETIME2 NULL,
        [Pokemon] NVARCHAR(100) NULL,
        [Tier] INT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    CREATE UNIQUE INDEX idx_bot_raids_guid ON [dbo].[bot_raids]([Guid]);
    CREATE INDEX idx_bot_raids_date_start ON [dbo].[bot_raids]([DateStart]);
    CREATE INDEX idx_bot_raids_date_end ON [dbo].[bot_raids]([DateEnd]);
    CREATE INDEX idx_bot_raids_pokemon ON [dbo].[bot_raids]([Pokemon]);
    CREATE INDEX idx_bot_raids_tier ON [dbo].[bot_raids]([Tier]);
    PRINT 'Table bot_raids created successfully';
END
GO

-- Create bot_pokemon table (replaces Datastore Pokemon collection)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bot_pokemon]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[bot_pokemon] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(255) NOT NULL,
        [Number] INT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    CREATE UNIQUE INDEX idx_bot_pokemon_name ON [dbo].[bot_pokemon]([Name]);
    CREATE INDEX idx_bot_pokemon_number ON [dbo].[bot_pokemon]([Number]);
    PRINT 'Table bot_pokemon created successfully';
END
GO

-- Create bot_raid_participants table (for tracking raid participants)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bot_raid_participants]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[bot_raid_participants] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [RaidGuid] UNIQUEIDENTIFIER NOT NULL,
        [DiscordId] NVARCHAR(100) NOT NULL,
        [Nickname] NVARCHAR(255) NULL,
        [ExtraPlayers] INT DEFAULT 0,
        [JoinedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY ([RaidGuid]) REFERENCES [dbo].[bot_raids]([Guid]) ON DELETE CASCADE
    );
    CREATE INDEX idx_bot_raid_participants_raid_guid ON [dbo].[bot_raid_participants]([RaidGuid]);
    CREATE INDEX idx_bot_raid_participants_discord_id ON [dbo].[bot_raid_participants]([DiscordId]);
    PRINT 'Table bot_raid_participants created successfully';
END
GO

-- Create bot_logs table (for storing bot-specific logs if needed)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bot_logs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[bot_logs] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Level] NVARCHAR(20) NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [DiscordId] NVARCHAR(100) NULL,
        [ChannelId] NVARCHAR(100) NULL,
        [GuildId] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    CREATE INDEX idx_bot_logs_level ON [dbo].[bot_logs]([Level]);
    CREATE INDEX idx_bot_logs_discord_id ON [dbo].[bot_logs]([DiscordId]);
    CREATE INDEX idx_bot_logs_created_at ON [dbo].[bot_logs]([CreatedAt]);
    PRINT 'Table bot_logs created successfully';
END
GO

-- Insert some sample Pokemon data
INSERT INTO [dbo].[bot_pokemon] ([Name], [Number]) VALUES
    ('Bulbasaur', 1),
    ('Ivysaur', 2),
    ('Venusaur', 3),
    ('Charmander', 4),
    ('Charmeleon', 5),
    ('Charizard', 6),
    ('Squirtle', 7),
    ('Wartortle', 8),
    ('Blastoise', 9),
    ('Caterpie', 10),
    ('Metapod', 11),
    ('Butterfree', 12),
    ('Weedle', 13),
    ('Kakuna', 14),
    ('Beedrill', 15),
    ('Pidgey', 16),
    ('Pidgeotto', 17),
    ('Pidgeot', 18),
    ('Rattata', 19),
    ('Raticate', 20);

PRINT 'Bot database schema creation complete';
GO
