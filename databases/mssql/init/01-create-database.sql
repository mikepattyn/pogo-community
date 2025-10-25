-- POGO Community Database - Create Database
-- This script creates the shared database for API and Bot

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'pogo')
BEGIN
    CREATE DATABASE pogo;
    PRINT 'Database pogo created successfully';
END
ELSE
BEGIN
    PRINT 'Database pogo already exists';
END
GO

USE pogo;
GO

