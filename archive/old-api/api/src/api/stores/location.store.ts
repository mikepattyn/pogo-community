import { injectable } from 'inversify';
import { Promise } from 'mssql';
import { DataLocation } from '../models/dbmodels/classes/DataLocation';
import { IDataLocation } from '../models/dbmodels/interfaces/IDataLocation';

const { poolPromise } = require('./../sqlDb');

@injectable()
export class LocationStore {
  async getById(id: number) {
    try {
      return new Promise((resolve: (value: unknown) => void, reject: (reason?: unknown) => void) => {
        poolPromise.query(
          DataLocation.GetById(),
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

  async post(dataLocation: IDataLocation) {
    try {
      return new Promise((resolve: (value: unknown) => void, reject: (reason?: unknown) => void) => {
        poolPromise.query(
          DataLocation.Insert(),
          [dataLocation.Latitude, dataLocation.Longtitude],
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
