import { IDataPlayer, Teams } from '../interfaces/IDataPlayer';

export class DataPlayer implements IDataPlayer {
  Id: number;
  DiscordId: string;
  FirstName: string | null;
  Nickname: string | null;
  Level: number | null;
  Team: Teams | null;
  DateJoined: Date;

  constructor(dataPlayer: IDataPlayer) {
    this.Id = dataPlayer.Id;
    this.DiscordId = dataPlayer.DiscordId;
    this.FirstName = dataPlayer.FirstName;
    this.Nickname = dataPlayer.Nickname;
    this.Level = dataPlayer.Level;
    this.Team = dataPlayer.Team;
    this.DateJoined = dataPlayer.DateJoined;
  }

  static Insert() {
    return 'INSERT INTO Players (DiscordId, FirstName, Nickname, Level, Team, DateJoined) VALUES (?, ?, ?, ?, ?, ?)';
  }
  static GetByDiscordId() {
    return 'SELECT * FROM Players WHERE DiscordId = ?';
  }
}
