import { injectable } from 'inversify';
import sql from 'mssql';

@injectable()
export class RaidStore {
  private config: sql.config = {
    server: process.env.DB_SERVER || 'localhost',
    port: parseInt(process.env.DB_PORT || '1433'),
    database: process.env.DB_NAME || 'pogo',
    user: process.env.DB_USER || 'sa',
    password: process.env.DB_PASSWORD || 'YourStrong@Passw0rd',
    options: {
      encrypt: false,
      trustServerCertificate: true
    }
  };

  async insert(raid: Raid): Promise<void> {
    try {
      const pool = await sql.connect(this.config);

      const request = pool.request();
      request.input('guid', sql.UniqueIdentifier, raid.Guid);
      request.input('dateEnd', sql.DateTime2, raid.DateEnd);
      request.input('dateStart', sql.DateTime2, raid.DateStart);
      request.input('pokemon', sql.NVarChar(100), raid.Pokemon);
      request.input('tier', sql.Int, raid.Tier);

      await request.query(`
        INSERT INTO bot_raids (Guid, DateEnd, DateStart, Pokemon, Tier)
        VALUES (@guid, @dateEnd, @dateStart, @pokemon, @tier)
      `);

      await pool.close();
    } catch (error) {
      console.error('Error inserting raid:', error);
      throw error;
    }
  }

  async get(guid: string): Promise<Raid | null> {
    try {
      const pool = await sql.connect(this.config);

      const request = pool.request();
      request.input('guid', sql.UniqueIdentifier, guid);

      const result = await request.query(`
        SELECT Id, Guid, DateEnd, DateStart, Pokemon, Tier, CreatedAt, UpdatedAt
        FROM bot_raids
        WHERE Guid = @guid
      `);

      await pool.close();

      if (result.recordset.length === 0) {
        return null;
      }

      const record = result.recordset[0];
      return {
        Guid: record.Guid,
        DateEnd: record.DateEnd,
        DateStart: record.DateStart,
        Pokemon: record.Pokemon,
        Tier: record.Tier
      };
    } catch (error) {
      console.error('Error getting raid:', error);
      throw error;
    }
  }

  async getAll(): Promise<Raid[]> {
    try {
      const pool = await sql.connect(this.config);

      const result = await pool.request().query(`
        SELECT Id, Guid, DateEnd, DateStart, Pokemon, Tier, CreatedAt, UpdatedAt
        FROM bot_raids
        ORDER BY DateStart DESC
      `);

      await pool.close();

      return result.recordset.map(record => ({
        Guid: record.Guid,
        DateEnd: record.DateEnd,
        DateStart: record.DateStart,
        Pokemon: record.Pokemon,
        Tier: record.Tier
      }));
    } catch (error) {
      console.error('Error getting all raids:', error);
      throw error;
    }
  }

  async update(raid: Raid): Promise<void> {
    try {
      const pool = await sql.connect(this.config);

      const request = pool.request();
      request.input('guid', sql.UniqueIdentifier, raid.Guid);
      request.input('dateEnd', sql.DateTime2, raid.DateEnd);
      request.input('dateStart', sql.DateTime2, raid.DateStart);
      request.input('pokemon', sql.NVarChar(100), raid.Pokemon);
      request.input('tier', sql.Int, raid.Tier);

      await request.query(`
        UPDATE bot_raids
        SET DateEnd = @dateEnd, DateStart = @dateStart, Pokemon = @pokemon, Tier = @tier, UpdatedAt = GETUTCDATE()
        WHERE Guid = @guid
      `);

      await pool.close();
    } catch (error) {
      console.error('Error updating raid:', error);
      throw error;
    }
  }

  async delete(guid: string): Promise<void> {
    try {
      const pool = await sql.connect(this.config);

      const request = pool.request();
      request.input('guid', sql.UniqueIdentifier, guid);

      await request.query(`
        DELETE FROM bot_raids WHERE Guid = @guid
      `);

      await pool.close();
    } catch (error) {
      console.error('Error deleting raid:', error);
      throw error;
    }
  }
}

export class Raid {
  Guid: string | null = null;
  DateEnd: Date | null = null;
  GymName: string | null = null;
  IsHatched: boolean | null = null;
  PokemonName: string | null = null;
  Tiers: number | null = null;
}
