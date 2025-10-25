export interface IDataAccount {
    Id: number
    PlayerId: number
    Password: string
    DateJoined: Date
    WrongAttempts: number
    LockedOut: Date | null
    Email: string
}