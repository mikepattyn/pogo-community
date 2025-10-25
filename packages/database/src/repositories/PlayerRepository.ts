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

  async create(player: Partial<IPlayer>): Promise<number> {
    try {
      const pool = await getConnection();
      const result = await pool
        .request()
        .input('discordId', mssql.VarChar, player.DiscordId)
        .input('firstName', mssql.VarChar, player.FirstName)
        .input('nickname', mssql.VarChar, player.Nickname)
        .input('level', mssql.Int, player.Level)
        .input('team', mssql.Int, player.Team)
        .input('dateJoined', mssql.DateTime, player.DateJoined || new Date())
        .query(
          'INSERT INTO Players (DiscordId, FirstName, Nickname, Level, Team, DateJoined) OUTPUT INSERTED.Id VALUES (@discordId, @firstName, @nickname, @level, @team, @dateJoined)'
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
        .input('firstName', mssql.VarChar, player.FirstName)
        .input('nickname', mssql.VarChar, player.Nickname)
        .input('level', mssql.Int, player.Level)
        .input('team', mssql.Int, player.Team)
        .query(
          'UPDATE Players SET FirstName = @firstName, Nickname = @nickname, Level = @level, Team = @team WHERE Id = @id'
        );

      return result.rowsAffected[0] > 0;
    } catch (error) {
      console.error('Error in update:', error);
      throw error;
    }
  }
}

