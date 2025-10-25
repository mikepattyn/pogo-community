import { IDataAccount } from "../interfaces/IDataAccount";

export class DataAccount implements IDataAccount {
    Id: number;
    PlayerId: number;
    Password: string;
    DateJoined: Date;
    WrongAttempts: number;
    LockedOut: Date | null;
    Email: string;

    constructor(dataAccount: IDataAccount) {
        this.Id = dataAccount.Id;
        this.PlayerId = dataAccount.PlayerId;
        this.Password = dataAccount.Password;
        this.DateJoined = dataAccount.DateJoined;
        this.WrongAttempts = dataAccount.WrongAttempts;
        this.LockedOut = dataAccount.LockedOut;
        this.Email = dataAccount.Email;
    }

    static Insert() {
        return "INSERT INTO Accounts (PlayerId, Password, DateJoined, Email) VALUES (?, ?, ?, ?)"
    }

    static GetByPlayerId() {
        return "SELECT * FROM Accounts WHERE PlayerId= ?"
    }

    static GetByEmail() {
        return "SELECT * FROM Accounts WHERE Email= ?"
    }
}