import { injectable } from 'inversify';
import * as mssql from 'mssql';
import { getConnection } from '../config/mssql.config';
import { IPlayer, Player } from '../models/Player';
import { MssqlQueryResult } from '../types/mssql.types';

@injectable()
export class PlayerRepository {
  async getByDiscordId(discordId: string): Promise<IPlayer | null> {
    try {
      const pool = await getConnection();
      const result: MssqlQueryResult<IPlayer> = await pool
        .request()
        .input('discordId', mssql.VarChar, discordId)
        .query('SELECT * FROM Players WHERE DiscordId = @discordId');

      return result.recordset.length > 0 ? result.recordset[0] : null;
    } catch (error) {
      console.error('Error in getByDiscordId:', error);
      throw error;
    }
  }

  async getByName(name: string): Promise<IPlayer | null> {
    try {
      const pool = await getConnection();
      const result: MssqlQueryResult<IPlayer> = await pool
        .request()
        .input('name', mssql.VarChar, name)
        .query('SELECT * FROM Players WHERE Name = @name');

      return result.recordset.length > 0 ? result.recordset[0] : null;
    } catch (error) {
      console.error('Error in getByName:', error);
      throw error;
    }
  }

  async getByTrainerCode(trainerCode: string): Promise<IPlayer | null> {
    try {
      const pool = await getConnection();
      const result: MssqlQueryResult<IPlayer> = await pool
        .request()
        .input('trainerCode', mssql.VarChar, trainerCode)
        .query('SELECT * FROM Players WHERE TrainerCode = @trainerCode');

      return result.recordset.length > 0 ? result.recordset[0] : null;
    } catch (error) {
      console.error('Error in getByTrainerCode:', error);
      throw error;
    }
  }

  async create(player: Partial<IPlayer>): Promise<number> {
    try {
      const pool = await getConnection();
      const result = await pool
        .request()
        .input('name', mssql.VarChar, player.Name)
        .input('team', mssql.VarChar, player.Team)
        .input('level', mssql.Int, player.Level)
        .input('trainerCode', mssql.VarChar, player.TrainerCode)
        .input('discordId', mssql.VarChar, player.DiscordId)
        .query(
          'INSERT INTO Players (Name, Team, Level, TrainerCode, DiscordId) OUTPUT INSERTED.Id VALUES (@name, @team, @level, @trainerCode, @discordId)'
        );

      return result.recordset[0].Id;
    } catch (error) {
      console.error('Error in create:', error);
      throw error;
    }
  }

  async update(id: number, player: Partial<IPlayer>): Promise<boolean> {
    try {
      const pool = await getConnection();
      const result = await pool
        .request()
        .input('id', mssql.Int, id)
        .input('name', mssql.VarChar, player.Name)
        .input('team', mssql.VarChar, player.Team)
        .input('level', mssql.Int, player.Level)
        .input('trainerCode', mssql.VarChar, player.TrainerCode)
        .input('discordId', mssql.VarChar, player.DiscordId)
        .query(
          'UPDATE Players SET Name = @name, Team = @team, Level = @level, TrainerCode = @trainerCode, DiscordId = @discordId, UpdatedAt = GETDATE() WHERE Id = @id'
        );

      return result.rowsAffected[0] > 0;
    } catch (error) {
      console.error('Error in update:', error);
      throw error;
    }
  }
}

