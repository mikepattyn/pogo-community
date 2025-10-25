import { Raid, HatchedRaid } from './classes/Raid';
import { isNullOrUndefined } from 'util';

export class RaidViewModel {
  Raid: Raid;
  constructor(raid: Raid) {
    this.Raid = new HatchedRaid(
      raid.Pokemon,
      raid.Gym,
      raid.Tiers,
      raid.TimeRemaining
    );
  }
}
