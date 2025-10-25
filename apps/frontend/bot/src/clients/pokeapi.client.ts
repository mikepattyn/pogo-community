import { ApiClient } from './http.client';

export class PokeapiClient {
  private baseUrl = 'http://pokeapi.co/api/v2';

  constructor() {}

  async getGeneration(gen: number) {
    const client = new ApiClient();
    const request = await client.get(
      `${this.baseUrl}/${PokeApiEndPoints.Generation.toString()}/${gen}`
    );
    return request;
  }
}

export enum PokeApiEndPoints {
  Generation = 'generation',
}
