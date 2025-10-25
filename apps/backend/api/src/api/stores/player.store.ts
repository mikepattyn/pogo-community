import { Promise } from 'mssql';
import { IDataPlayer } from '../models/dbmodels/interfaces/IDataPlayer';
import { DataPlayer } from '../models/dbmodels/classes/DataPlayer';
import { injectable } from 'inversify';

const { poolPromise } = require('./../sqlDb');

@injectable()
export class PlayerStore {
  async post(dataPlayer: IDataPlayer) {
    return new Promise((resolve: any, reject: any) => {
      poolPromise.query(
        DataPlayer.Insert(),
        [
          dataPlayer.DiscordId,
          dataPlayer.FirstName,
          dataPlayer.Nickname,
          dataPlayer.Level,
          dataPlayer.Team,
          dataPlayer.DateJoined,
        ],
        (error: any, results: any, fields: any) => {
          if (error) {
            console.log('Error: ', error);
            reject(error);
          }
          if (results) {
            resolve(results.insertId);
          }
        }
      );
    });
  }
  async get(id: number) {
    return new Promise((resolve: any, reject: any) => {
      poolPromise.query(
        DataPlayer.GetByDiscordId(),
        [id],
        (error: any, results: any, fields: any) => {
          if (error) {
            console.log('Error: ', error);
            reject(error);
          }
          if (results && results.length > 1) {
            resolve(results);
          } else {
            resolve(null);
          }
        }
      );
    });
  }
}
