import { IDataRaid } from '../models/dbmodels/interfaces/IDataRaid';

import { DataRaid } from '../models/dbmodels/classes/DataRaid';

import { injectable } from 'inversify';
import { Promise } from 'mssql';

const { poolPromise } = require('./../sqlDb');

@injectable()
export class RaidStore {
  async getByMessageId(id: string) {
    try {
      return new Promise((resolve: (value: unknown) => void, reject: (reason?: unknown) => void) => {
        poolPromise.query(
          DataRaid.GetByMessageId(),
          [id],
          (error: unknown, results: unknown, _fields: unknown) => {
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
      return new Promise((resolve: (value: unknown) => void, reject: (reason?: unknown) => void) => {
        poolPromise.query(
          DataRaid.AddPlayerToRaid(),
          [raidId, playerId, new Date()],
          (error: unknown, results: unknown, _fields: unknown) => {
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
      return new Promise((resolve: (value: unknown) => void, reject: (reason?: unknown) => void) => {
        poolPromise.query(
          DataRaid.Insert(),
          [
            dataRaid.MessageId,
            dataRaid.GymId,
            dataRaid.Tiers,
            dataRaid.TimeRemaining,
          ],
          (error: unknown, results: unknown, _fields: unknown) => {
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
