export interface IDataPlayer {
    Id: number;
    DiscordId: string;
    FirstName: string | null;
    Nickname: string | null;
    Level: number | null;
    Team: Teams | null;
    DateJoined: string;
}

export enum Teams {
    Instinct,
    Mystic,
    Valor
}