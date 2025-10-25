/**
 * Type definitions for MySQL query results
 */

export interface MysqlFieldInfo {
  catalog: string;
  db: string;
  table: string;
  orgTable: string;
  name: string;
  orgName: string;
  charsetNr: number;
  length: number;
  type: number;
  flags: number;
  decimals: number;
  default?: string;
  zeroFill: boolean;
  protocol41: boolean;
}

export interface MysqlInsertResult {
  fieldCount: number;
  affectedRows: number;
  insertId: number;
  serverStatus: number;
  warningCount: number;
  message: string;
  protocol41: boolean;
  changedRows: number;
}

export type MysqlSelectResult<T = any> = T[];

export type MysqlQueryResult<T = any> = MysqlSelectResult<T> | MysqlInsertResult;

