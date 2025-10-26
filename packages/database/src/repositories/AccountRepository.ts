import { injectable } from 'inversify';
import * as mssql from 'mssql';
import { getConnection } from '../config/mssql.config';
import { IAccount, Account } from '../models/Account';
import { MssqlQueryResult } from '../types/mssql.types';

@injectable()
export class AccountRepository {
  async getByPlayerId(playerId: number): Promise<IAccount | null> {
    try {
      const pool = await getConnection();
      const result: MssqlQueryResult<IAccount> = await pool
        .request()
        .input('playerId', mssql.Int, playerId)
        .query('SELECT * FROM Accounts WHERE PlayerId = @playerId');

      return result.recordset.length > 0 ? result.recordset[0] : null;
    } catch (error) {
      console.error('Error in getByPlayerId:', error);
      throw error;
    }
  }

  async getByEmail(email: string): Promise<IAccount | null> {
    try {
      const pool = await getConnection();
      const result: MssqlQueryResult<IAccount> = await pool
        .request()
        .input('email', mssql.VarChar, email)
        .query('SELECT * FROM Accounts WHERE Email = @email');

      return result.recordset.length > 0 ? result.recordset[0] : null;
    } catch (error) {
      console.error('Error in getByEmail:', error);
      throw error;
    }
  }

  async create(account: Partial<IAccount>): Promise<number> {
    try {
      const pool = await getConnection();
      const result = await pool
        .request()
        .input('playerId', mssql.Int, account.PlayerId)
        .input('password', mssql.VarChar, account.Password)
        .input('dateJoined', mssql.DateTime, account.DateJoined || new Date())
        .input('email', mssql.VarChar, account.Email)
        .query(
          'INSERT INTO Accounts (PlayerId, Password, DateJoined, Email) OUTPUT INSERTED.Id VALUES (@playerId, @password, @dateJoined, @email)'
        );

      return result.recordset[0].Id;
    } catch (error) {
      console.error('Error in create:', error);
      throw error;
    }
  }

  async update(id: number, account: Partial<IAccount>): Promise<boolean> {
    try {
      const pool = await getConnection();
      const result = await pool
        .request()
        .input('id', mssql.Int, id)
        .input('wrongAttempts', mssql.Int, account.WrongAttempts)
        .input('lockedOut', mssql.DateTime, account.LockedOut)
        .query(
          'UPDATE Accounts SET WrongAttempts = @wrongAttempts, LockedOut = @lockedOut WHERE Id = @id'
        );

      return result.rowsAffected[0] > 0;
    } catch (error) {
      console.error('Error in update:', error);
      throw error;
    }
  }
}
