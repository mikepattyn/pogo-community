import { isNullOrUndefined } from "util";

export interface IRegistrationSettings {
    Email?: string;
    Password?: string;
    DiscordId?: string;
    FirstName?: string;
    Nickname?: string;
    Level?: number;
    Team?: number;
    IsRegistered?: boolean;
}