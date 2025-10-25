import { injectable } from 'inversify';
import { Promise } from 'mssql';
import { IDataGym } from '../models/dbmodels/interfaces/IDataGym';
import { DataGym } from '../models/dbmodels/classes/DataGym';

const { poolPromise } = require('./../sqlDb');

@injectable()
export class GymStore {
  async getById(id: number) {
    try {
      return new Promise((resolve: (value: unknown) => void, reject: (reason?: unknown) => void) => {
        poolPromise.query(
          DataGym.GetById(),
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

  async post(dataGym: IDataGym) {
    try {
      return new Promise((resolve: (value: unknown) => void, reject: (reason?: unknown) => void) => {
        poolPromise.query(
          DataGym.Insert(),
          [dataGym.Name, dataGym.LocationId],
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
  async getAll() {
    try {
      return new Promise((resolve: (value: unknown) => void, reject: (reason?: unknown) => void) => {
        poolPromise.query(
          DataGym.GetAllWithLocations(),
          null,
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
}
