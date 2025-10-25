import { injectable } from 'inversify';
import { Promise } from 'mssql';
import { DataAccount } from '../models/dbmodels/classes/DataAccount';

const { poolPromise } = require('./../sqlDb');

@injectable()
export class AuthStore {
  async getByPlayerId(playerId: number) {
    try {
      return new Promise((resolve: any, reject: any) => {
        poolPromise.query(
          DataAccount.GetByPlayerId(),
          [playerId],
          (error: any, results: any, _fields: any) => {
            if (error) reject(error);
            if (results) {
              resolve(results);
            }
          }
        );
      });
    } catch (error) {
      console.log(error);
    }
  }

  async getByEmail(email: string) {
    try {
      return new Promise((resolve: any, reject: any) => {
        poolPromise.query(
          DataAccount.GetByEmail(),
          [email],
          (error: any, results: any, _fields: any) => {
            if (error) reject(error);
            if (results && results.length == 1) {
              resolve(results[0]);
            } else {
              reject();
            }
          }
        );
      });
    } catch (error) {
      console.log(error);
    }
  }

  async post(dataAccount: any) {
    try {
      return new Promise((resolve: any, reject: any) => {
        poolPromise.query(
          DataAccount.Insert(),
          [
            dataAccount.PlayerId,
            dataAccount.Password,
            dataAccount.DateJoined,
            dataAccount.Email,
          ],
          (error: any, results: any, _fields: any) => {
            if (error) reject(error);
            if (results) {
              resolve(results.insertId);
            }
          }
        );
      });
    } catch (error) {
      console.log(error);
    }
  }
}
