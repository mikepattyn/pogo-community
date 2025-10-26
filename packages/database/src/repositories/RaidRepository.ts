import { injectable } from 'inversify';
import * as mssql from 'mssql';
import { getConnection } from '../config/mssql.config';
import { IRaid } from '../models/Raid';
import { MssqlQueryResult } from '../types/mssql.types';

@injectable()
export class RaidRepository {
  async getById(id: number): Promise<IRaid | null> {
    try {
      const pool = await getConnection();
      const result: MssqlQueryResult<IRaid> = await pool
        .request()
        .input('id', mssql.Int, id)
        .query('SELECT * FROM Raids WHERE Id = @id');

      return result.recordset.length > 0 ? result.recordset[0] : null;
    } catch (error) {
      console.error('Error in getById:', error);
      throw error;
    }
  }

  async create(raid: Partial<IRaid>): Promise<number> {
    try {
      const pool = await getConnection();
      const result = await pool
        .request()
        .input('gymId', mssql.Int, raid.GymId)
        .input('pokemon', mssql.VarChar, raid.Pokemon)
        .input('level', mssql.Int, raid.Level)
        .input('startTime', mssql.DateTime, raid.StartTime)
        .input('endTime', mssql.DateTime, raid.EndTime)
        .input('createdBy', mssql.Int, raid.CreatedBy)
        .query(
          'INSERT INTO Raids (GymId, Pokemon, Level, StartTime, EndTime, CreatedBy) OUTPUT INSERTED.Id VALUES (@gymId, @pokemon, @level, @startTime, @endTime, @createdBy)'
        );

      return result.recordset[0].Id;
    } catch (error) {
      console.error('Error in create:', error);
      throw error;
    }
  }
}
