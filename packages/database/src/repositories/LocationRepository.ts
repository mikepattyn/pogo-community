import { injectable } from 'inversify';
import * as mssql from 'mssql';
import { getConnection } from '../config/mssql.config';
import { ILocation } from '../models/Location';
import { MssqlQueryResult } from '../types/mssql.types';

@injectable()
export class LocationRepository {
  async getById(id: number): Promise<ILocation | null> {
    try {
      const pool = await getConnection();
      const result: MssqlQueryResult<ILocation> = await pool
        .request()
        .input('id', mssql.Int, id)
        .query('SELECT * FROM Locations WHERE Id = @id');

      return result.recordset.length > 0 ? result.recordset[0] : null;
    } catch (error) {
      console.error('Error in getById:', error);
      throw error;
    }
  }

  async create(location: Partial<ILocation>): Promise<number> {
    try {
      const pool = await getConnection();
      const result = await pool
        .request()
        .input('latitude', mssql.VarChar, location.Latitude)
        .input('longtitude', mssql.VarChar, location.Longtitude)
        .query(
          'INSERT INTO Locations (Latitude, Longtitude) OUTPUT INSERTED.Id VALUES (@latitude, @longtitude)'
        );

      return result.recordset[0].Id;
    } catch (error) {
      console.error('Error in create:', error);
      throw error;
    }
  }
}

