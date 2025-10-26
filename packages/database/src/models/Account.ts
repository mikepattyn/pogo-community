export interface IAccount {
  Id: number;
  PlayerId: number;
  Password: string;
  DateJoined: Date;
  WrongAttempts: number;
  LockedOut: Date | null;
  Email: string;
}

export class Account implements IAccount {
  Id: number;
  PlayerId: number;
  Password: string;
  DateJoined: Date;
  WrongAttempts: number;
  LockedOut: Date | null;
  Email: string;

  constructor(data: IAccount) {
    this.Id = data.Id;
    this.PlayerId = data.PlayerId;
    this.Password = data.Password;
    this.DateJoined = data.DateJoined;
    this.WrongAttempts = data.WrongAttempts;
    this.LockedOut = data.LockedOut;
    this.Email = data.Email;
  }
}
