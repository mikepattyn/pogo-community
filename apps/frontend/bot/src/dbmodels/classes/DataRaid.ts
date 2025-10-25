import { IDataRaid } from "../interfaces/IDataRaid";
export class DataRaid implements IDataRaid {
    Id: number;
    MessageId: string;
    GymId: number;
    CreatedAt: Date;
    Tiers: number;
    TimeRemaining: number;

    constructor(dataRaid: IDataRaid) {
        this.Id = dataRaid.Id
        this.MessageId = dataRaid.MessageId
        this.GymId = dataRaid.GymId
        this.CreatedAt = dataRaid.CreatedAt
        this.Tiers = dataRaid.Tiers
        this.TimeRemaining = dataRaid.TimeRemaining
    }
    static GetByMessageId() {
        return `SELECT * FROM Raids
                    INNER JOIN Gyms ON Raids.GymId = Gyms.Id
                    INNER JOIN Locations ON Gyms.LocationId = Locations.Id
                WHERE Raids.MessageId = ?`;
    }
    static Insert() {
        return `INSERT INTO Raids (MessageId, GymId, Tiers, TimeRemaining) VALUES (?, ?, ?, ?)`;
    }
    static AddPlayerToRaid() {
        return `INSERT INTO RaidPlayers (RaidId, PlayerId, Joined) VALUES (?, ?, ?)`;
    }
}