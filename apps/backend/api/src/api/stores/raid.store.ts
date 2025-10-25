import { IDataRaid } from '../models/dbmodels/interfaces/IDataRaid';

import { DataRaid } from '../models/dbmodels/classes/DataRaid';

import { injectable } from 'inversify';
import { Promise } from 'mssql';

const { poolPromise } = require('./../sqlDb');

@injectable()
export class RaidStore {
  async getByMessageId(id: string) {
    try {
      return new Promise((resolve: any, reject: any) => {
        poolPromise.query(
          DataRaid.GetByMessageId(),
          [id],
          (error: any, results: any, fields: any) => {
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
  async addPlayer(raidId: string, playerId: string) {
    try {
      return new Promise((resolve: any, reject: any) => {
        poolPromise.query(
          DataRaid.AddPlayerToRaid(),
          [raidId, playerId, new Date()],
          (error: any, results: any, fields: any) => {
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
  async post(dataRaid: IDataRaid) {
    try {
      return new Promise((resolve: any, reject: any) => {
        poolPromise.query(
          DataRaid.Insert(),
          [
            dataRaid.MessageId,
            dataRaid.GymId,
            dataRaid.Tiers,
            dataRaid.TimeRemaining,
          ],
          (error: any, results: any, fields: any) => {
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
