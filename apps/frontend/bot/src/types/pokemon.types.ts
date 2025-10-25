/**
 * Type definitions for Pokemon-related data structures
 */

export interface PokemonCounter {
  name: string;
  // Add other properties as needed based on your data structure
  [key: string]: any;
}

export type PokemonCounterResult = PokemonCounter[] | null;

