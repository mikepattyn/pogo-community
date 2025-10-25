-- POGO Community API Database - Sample Seed Data
-- This script inserts some initial data for testing purposes

USE pogo_api;

-- Insert sample locations
INSERT INTO Locations (Latitude, Longtitude, Name, Address) VALUES
(50.9513, 3.1266, 'Roeselare Centrum', 'Grote Markt, Roeselare'),
(50.9145, 3.2134, 'Izegem Centrum', 'Korenmarkt, Izegem'),
(50.9833, 3.0833, 'Hooglede Centrum', 'Marktplein, Hooglede');

-- Note: Additional seed data can be added here as needed
-- Players, Accounts, Gyms, and Raids should be created through the application

