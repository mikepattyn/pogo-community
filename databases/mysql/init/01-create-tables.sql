-- POGO Community API Database Schema
-- This script creates the necessary tables for the API backend

USE pogo_api;

-- Create Accounts table
CREATE TABLE IF NOT EXISTS Accounts (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    PlayerId INT NOT NULL,
    Password VARCHAR(255) NOT NULL,
    DateJoined DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    WrongAttempts INT DEFAULT 0,
    LockedOut DATETIME NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    INDEX idx_player_id (PlayerId),
    INDEX idx_email (Email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create Players table
CREATE TABLE IF NOT EXISTS Players (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Team VARCHAR(50),
    Level INT DEFAULT 1,
    TrainerCode VARCHAR(20),
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_name (Name),
    INDEX idx_trainer_code (TrainerCode)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create Locations table
CREATE TABLE IF NOT EXISTS Locations (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Latitude DECIMAL(10, 8) NOT NULL,
    Longtitude DECIMAL(11, 8) NOT NULL,
    Name VARCHAR(255),
    Address VARCHAR(255),
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_coordinates (Latitude, Longtitude)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create Gyms table
CREATE TABLE IF NOT EXISTS Gyms (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    LocationId INT NOT NULL,
    ExGym BOOLEAN DEFAULT FALSE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (LocationId) REFERENCES Locations(Id) ON DELETE CASCADE,
    INDEX idx_name (Name),
    INDEX idx_location (LocationId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create Raids table
CREATE TABLE IF NOT EXISTS Raids (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    GymId INT NOT NULL,
    Pokemon VARCHAR(100) NOT NULL,
    Level INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    CreatedBy INT,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (GymId) REFERENCES Gyms(Id) ON DELETE CASCADE,
    FOREIGN KEY (CreatedBy) REFERENCES Players(Id) ON DELETE SET NULL,
    INDEX idx_gym (GymId),
    INDEX idx_start_time (StartTime),
    INDEX idx_end_time (EndTime)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create RaidParticipants table (many-to-many relationship)
CREATE TABLE IF NOT EXISTS RaidParticipants (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    RaidId INT NOT NULL,
    PlayerId INT NOT NULL,
    JoinedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (RaidId) REFERENCES Raids(Id) ON DELETE CASCADE,
    FOREIGN KEY (PlayerId) REFERENCES Players(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_raid_player (RaidId, PlayerId),
    INDEX idx_raid (RaidId),
    INDEX idx_player (PlayerId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

