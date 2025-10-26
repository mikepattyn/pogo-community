import { injectable } from 'inversify';
import { isNullOrUndefined } from 'util';
import { ApiClient } from '../clients/http.client';

@injectable()
export class PokemonService {
  async searchPokemonCounter(name: string) {
    const client = new ApiClient();
    const request = await client.get(
      'http://www.mocky.io/v2/5dbe9df2330000f130a0e40b'
    );
    let pokemonCounters: Array<{ name: string }> | null = null;
    if (!isNullOrUndefined(request)) {
      pokemonCounters = request;
      console.log('Info: ', JSON.stringify(pokemonCounters));
    }
    const pokemon = pokemonCounters?.filter(
      (x: { name: string }) => x.name.toLowerCase() == name.toLowerCase()
    ) || [];
    if (!isNullOrUndefined(pokemon)) {
      return pokemon;
    } else {
      return null;
    }
  }
  async getCountersList() {
    const client = new ApiClient();
    const request = await client.get(
      'http://www.mocky.io/v2/5dbe9df2330000f130a0e40b'
    );
    if (!isNullOrUndefined(request)) {
      const list = request.map((x: { name: string }) => x.name);
      return list;
    } else {
      return null;
    }
  }
}

export interface PokemonCounter {
  name: string;
  counters: PokemonWithAttackCounter[];
  thumbnail: string;
}
export interface PokemonWithAttackCounter {
  name: string;
  attacks: string[][];
}
