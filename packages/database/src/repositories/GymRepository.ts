import { injectable } from 'inversify';
import * as mssql from 'mssql';
import { getConnection } from '../config/mssql.config';
import { IGym } from '../models/Gym';
import { MssqlQueryResult } from '../types/mssql.types';

@injectable()
export class GymRepository {
  async getById(id: number): Promise<IGym | null> {
    try {
      const pool = await getConnection();
      const result: MssqlQueryResult<IGym> = await pool
        .request()
        .input('id', mssql.Int, id)
        .query('SELECT * FROM Gyms WHERE Id = @id');

      return result.recordset.length > 0 ? result.recordset[0] : null;
    } catch (error) {
      console.error('Error in getById:', error);
      throw error;
    }
  }

  async getAllWithLocations(): Promise<any[]> {
    try {
      const pool = await getConnection();
      const result = await pool.request().query(`
        SELECT Gyms.Name, Locations.Latitude, Locations.Longtitude 
        FROM Gyms 
        INNER JOIN Locations ON Gyms.LocationId = Locations.Id
        WHERE NOT Gyms.Name = ''
      `);

      return result.recordset;
    } catch (error) {
      console.error('Error in getAllWithLocations:', error);
      throw error;
    }
  }

  async create(gym: Partial<IGym>): Promise<number> {
    try {
      const pool = await getConnection();
      const result = await pool
        .request()
        .input('name', mssql.VarChar, gym.Name)
        .input('locationId', mssql.Int, gym.LocationId)
        .query(
          'INSERT INTO Gyms (Name, LocationId) OUTPUT INSERTED.Id VALUES (@name, @locationId)'
        );

      return result.recordset[0].Id;
    } catch (error) {
      console.error('Error in create:', error);
      throw error;
    }
  }
}

