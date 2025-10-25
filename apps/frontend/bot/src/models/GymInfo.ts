export class GymInfo {
  titel: string | null;
  pokemon: string | null;
  private _time_left: string | null;
  get dtEnd() {
    const timeObjects = this._time_left!.split(':');
    let date = new Date();
    const totalSecondsLeft =
      Number(timeObjects[0]) * 60 * 60 +
      Number(timeObjects[1]) * 60 +
      Number(timeObjects[2]);
    date = new Date(date.setTime(date.getTime() + totalSecondsLeft * 1000));
    return date;
  }
  get time_left() {
    return this._time_left;
  }
  constructor(info: string[]) {
    this.titel = info[0];
    this.pokemon = info[1];
    this._time_left = info[2];
  }
  toString() {
    return `Found a ${this.pokemon} at${this.titel} - Ends at: ${this.dtEnd}`;
  }
}
