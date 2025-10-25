import { IRaid } from '../interfaces/IRaid';
import { IGym } from '../interfaces/IGym';

export class Raid implements IRaid {
  Pokemon: string;
  Gym: IGym;
  Tiers: number;
  TimeRemaining: number;

  constructor(
    pokemon: string,
    gym: IGym,
    tiers: number,
    timeRemaining: number
  ) {
    this.Pokemon = pokemon;
    this.Gym = gym;
    this.Tiers = tiers;
    this.TimeRemaining = timeRemaining;
  }
}

export class HatchedRaid extends Raid {
  Hatched: boolean;
  constructor(
    pokemon: string,
    gym: IGym,
    tiers: number,
    timeRemaining: number
  ) {
    super(pokemon, gym, tiers, timeRemaining);
    this.Hatched = this.Pokemon != null;
  }
}
