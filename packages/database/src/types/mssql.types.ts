import { IResult } from 'mssql';

// Re-export the built-in IResult type from mssql
export type MssqlQueryResult<T = any> = IResult<T>;

export interface MssqlInsertResult {
  rowsAffected: number[];
  recordset: Array<{ insertId?: number; id?: number }>;
}

export type MssqlSelectResult<T> = T[];

export interface QueryOptions {
  timeout?: number;
  stream?: boolean;
}

