-- POGO Community Bot Database - Create Database
-- This script creates the database for the Discord bot

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'pogo_bot')
BEGIN
    CREATE DATABASE pogo_bot;
    PRINT 'Database pogo_bot created successfully';
END
ELSE
BEGIN
    PRINT 'Database pogo_bot already exists';
END
GO

USE pogo_bot;
GO

