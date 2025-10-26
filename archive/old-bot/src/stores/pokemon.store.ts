import { injectable } from 'inversify';
import sql from 'mssql';
import axios from 'axios';

@injectable()
export class PokemonStore {
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

  async insert(pokemon: Pokemon[]): Promise<void> {
    try {
      const pool = await sql.connect(this.config);

      for (const mon of pokemon) {
        const request = pool.request();
        request.input('name', sql.NVarChar(255), mon.name);
        request.input('number', sql.Int, mon.number);

        await request.query(`
          IF NOT EXISTS (SELECT 1 FROM bot_pokemon WHERE Name = @name)
          BEGIN
            INSERT INTO bot_pokemon (Name, Number)
            VALUES (@name, @number)
          END
        `);
      }

      await pool.close();
    } catch (error) {
      console.error('Error inserting pokemon:', error);
      throw error;
    }
  }

  async searchByName(name: string): Promise<Pokemon | null> {
    try {
      const pool = await sql.connect(this.config);

      const request = pool.request();
      request.input('name', sql.NVarChar(255), name);

      const result = await request.query(`
        SELECT Id, Name, Number, CreatedAt, UpdatedAt
        FROM bot_pokemon
        WHERE Name = @name
      `);

      await pool.close();

      if (result.recordset.length === 0) {
        return null;
      }

      const record = result.recordset[0];
      return {
        name: record.Name,
        number: record.Number
      };
    } catch (error) {
      console.error('Error searching pokemon by name:', error);
      throw error;
    }
  }

  async get(id: number): Promise<Pokemon | null> {
    try {
      const pool = await sql.connect(this.config);

      const request = pool.request();
      request.input('id', sql.Int, id);

      const result = await request.query(`
        SELECT Id, Name, Number, CreatedAt, UpdatedAt
        FROM bot_pokemon
        WHERE Id = @id
      `);

      await pool.close();

      if (result.recordset.length === 0) {
        return null;
      }

      const record = result.recordset[0];
      return {
        name: record.Name,
        number: record.Number
      };
    } catch (error) {
      console.error('Error getting pokemon:', error);
      throw error;
    }
  }

  async getAll(): Promise<Pokemon[]> {
    try {
      const pool = await sql.connect(this.config);

      const result = await pool.request().query(`
        SELECT Id, Name, Number, CreatedAt, UpdatedAt
        FROM bot_pokemon
        ORDER BY Number
      `);

      await pool.close();

      return result.recordset.map(record => ({
        name: record.Name,
        number: record.Number
      }));
    } catch (error) {
      console.error('Error getting all pokemon:', error);
      throw error;
    }
  }

  async getNamesFromRapiAPI() {
    let retVal = null;
    await axios({
      method: 'GET',
      url: 'https://pokemon-go1.p.rapidapi.com/pokemon_names.json',
      headers: {
        'content-type': 'application/octet-stream',
        'x-rapidapi-host': 'pokemon-go1.p.rapidapi.com',
        'x-rapidapi-key': '4e23a33e0emsh55a6269c414188fp10370djsn6d976e399e42',
      },
    })
      .then((response: { data: unknown }) => {
        console.log(response);
        retVal = response.data;
      })
      .catch((error: unknown) => {
        console.log(error);
      });
    return retVal;
  }
}

export class Pokemon {
  name: string | null = null;
  number: number | null = null;
}
