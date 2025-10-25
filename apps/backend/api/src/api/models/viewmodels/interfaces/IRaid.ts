import { IGym } from './IGym';
export interface IRaid {
  Pokemon: string;
  Gym: IGym;
  Tiers: number;
  TimeRemaining: number;
}
