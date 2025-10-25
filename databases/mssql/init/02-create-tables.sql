-- POGO Community Database Schema
-- This script creates the necessary tables for API and Bot

USE pogo;
GO

-- Create Accounts table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Accounts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Accounts] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [PlayerId] INT NOT NULL,
        [Password] NVARCHAR(255) NOT NULL,
        [DateJoined] DATETIME NOT NULL DEFAULT GETDATE(),
        [WrongAttempts] INT DEFAULT 0,
        [LockedOut] DATETIME NULL,
        [Email] NVARCHAR(255) NOT NULL UNIQUE
    );
    CREATE INDEX idx_accounts_player_id ON [dbo].[Accounts]([PlayerId]);
    CREATE INDEX idx_accounts_email ON [dbo].[Accounts]([Email]);
    PRINT 'Table Accounts created successfully';
END
GO

-- Create Players table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Players]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Players] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(255) NOT NULL,
        [Team] NVARCHAR(50) NULL,
        [Level] INT DEFAULT 1,
        [TrainerCode] NVARCHAR(20) NULL,
        [DiscordId] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
    );
    CREATE INDEX idx_players_name ON [dbo].[Players]([Name]);
    CREATE INDEX idx_players_trainer_code ON [dbo].[Players]([TrainerCode]);
    CREATE INDEX idx_players_discord_id ON [dbo].[Players]([DiscordId]);
    PRINT 'Table Players created successfully';
END
GO

-- Create Locations table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Locations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Locations] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Latitude] DECIMAL(10, 8) NOT NULL,
        [Longtitude] DECIMAL(11, 8) NOT NULL,
        [Name] NVARCHAR(255) NULL,
        [Address] NVARCHAR(255) NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE()
    );
    CREATE INDEX idx_locations_coordinates ON [dbo].[Locations]([Latitude], [Longtitude]);
    PRINT 'Table Locations created successfully';
END
GO

-- Create Gyms table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Gyms]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Gyms] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(255) NOT NULL,
        [LocationId] INT NOT NULL,
        [ExGym] BIT DEFAULT 0,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations]([Id]) ON DELETE CASCADE
    );
    CREATE INDEX idx_gyms_name ON [dbo].[Gyms]([Name]);
    CREATE INDEX idx_gyms_location ON [dbo].[Gyms]([LocationId]);
    PRINT 'Table Gyms created successfully';
END
GO

-- Create Raids table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Raids]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Raids] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [GymId] INT NOT NULL,
        [Pokemon] NVARCHAR(100) NOT NULL,
        [Level] INT NOT NULL,
        [StartTime] DATETIME NOT NULL,
        [EndTime] DATETIME NOT NULL,
        [CreatedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([GymId]) REFERENCES [dbo].[Gyms]([Id]) ON DELETE CASCADE,
        FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Players]([Id]) ON DELETE SET NULL
    );
    CREATE INDEX idx_raids_gym ON [dbo].[Raids]([GymId]);
    CREATE INDEX idx_raids_start_time ON [dbo].[Raids]([StartTime]);
    CREATE INDEX idx_raids_end_time ON [dbo].[Raids]([EndTime]);
    PRINT 'Table Raids created successfully';
END
GO

-- Create RaidParticipants table (many-to-many relationship)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RaidParticipants]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RaidParticipants] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [RaidId] INT NOT NULL,
        [PlayerId] INT NOT NULL,
        [JoinedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([RaidId]) REFERENCES [dbo].[Raids]([Id]) ON DELETE CASCADE,
        FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([Id]) ON DELETE CASCADE,
        CONSTRAINT unique_raid_player UNIQUE ([RaidId], [PlayerId])
    );
    CREATE INDEX idx_raid_participants_raid ON [dbo].[RaidParticipants]([RaidId]);
    CREATE INDEX idx_raid_participants_player ON [dbo].[RaidParticipants]([PlayerId]);
    PRINT 'Table RaidParticipants created successfully';
END
GO

PRINT 'Database schema creation complete';
GO

