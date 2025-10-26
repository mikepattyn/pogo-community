import * as mssql from 'mssql';

export interface MssqlConfig {
  server: string;
  database: string;
  user: string;
  password: string;
  port?: number;
  options?: {
    encrypt?: boolean;
    trustServerCertificate?: boolean;
  };
}

export function createMssqlConfig(): MssqlConfig {
  return {
    server: process.env.MSSQL_HOST || 'localhost',
    database: process.env.MSSQL_DATABASE || 'pogo',
    user: process.env.MSSQL_USER || 'sa',
    password: process.env.MSSQL_PASSWORD || '',
    port: parseInt(process.env.MSSQL_PORT || '1433', 10),
    options: {
      encrypt: process.env.MSSQL_ENCRYPT === 'true',
      trustServerCertificate: process.env.MSSQL_TRUST_CERT !== 'false',
    },
  };
}

let pool: mssql.ConnectionPool | undefined;

export async function getConnection(): Promise<mssql.ConnectionPool> {
  if (!pool) {
    const config = createMssqlConfig();
    pool = new mssql.ConnectionPool(config);
    await pool.connect();
  }
  return pool;
}

export async function closeConnection(): Promise<void> {
  if (pool) {
    await pool.close();
    pool = undefined;
  }
}
