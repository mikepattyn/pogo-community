-- POGO Community Bot Database - Sample Seed Data
-- This script inserts some initial data for testing purposes

USE pogo_bot;
GO

-- Insert sample locations
IF NOT EXISTS (SELECT * FROM [dbo].[Locations] WHERE [Name] = 'Roeselare Centrum')
BEGIN
    INSERT INTO [dbo].[Locations] ([Latitude], [Longtitude], [Name], [Address]) VALUES
    ('50.9513', '3.1266', 'Roeselare Centrum', 'Grote Markt, Roeselare'),
    ('50.9145', '3.2134', 'Izegem Centrum', 'Korenmarkt, Izegem'),
    ('50.9833', '3.0833', 'Hooglede Centrum', 'Marktplein, Hooglede'),
    ('50.9667', '3.1167', 'Oekene Centrum', 'Dorpsplein, Oekene');
    
    PRINT 'Sample locations inserted successfully';
END
ELSE
BEGIN
    PRINT 'Sample locations already exist';
END
GO

-- Note: Additional seed data can be added here as needed
-- Players, Accounts, Gyms, and Raids should be created through the Discord bot

