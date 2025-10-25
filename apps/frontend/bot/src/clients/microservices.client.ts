import { inject, injectable } from 'inversify';
import { ApiClient } from './apiClient';
import { Logger } from '../logger';

export interface AccountDto {
  id: number;
  email: string;
  playerId: number;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
}

export interface PlayerDto {
  id: number;
  username: string;
  level: number;
  team: string;
  friendCode?: string;
  discordId?: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
}

export interface LocationDto {
  id: number;
  name: string;
  type: string;
  latitude: number;
  longitude: number;
  description?: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface GymDto {
  id: number;
  name: string;
  locationId: number;
  level: number;
  team?: string;
  isActive: boolean;
  isUnderAttack: boolean;
  isInRaid: boolean;
  motivationLevel: number;
  lastControlledByTeam?: string;
  lastControlledAt?: string;
  createdAt: string;
  updatedAt: string;
}

export interface RaidDto {
  id: number;
  gymId: number;
  pokemonSpecies: string;
  level: number;
  startTime: string;
  endTime: string;
  isActive: boolean;
  isCompleted: boolean;
  isCancelled: boolean;
  maxParticipants: number;
  currentParticipants: number;
  difficulty: string;
  weatherBoost?: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

@injectable()
export class MicroservicesClient {
  constructor(
    @inject(ApiClient) private apiClient: ApiClient,
    @inject('Logger') private logger: Logger
  ) {}

  // Account Service Methods
  async createAccount(email: string, password: string, playerId: number): Promise<AccountDto | null> {
    try {
      const response = await this.apiClient.post('/api/account', {
        email,
        password,
        playerId
      });
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to create account', error);
      return null;
    }
  }

  async login(email: string, password: string): Promise<{ token: string; account: AccountDto } | null> {
    try {
      const response = await this.apiClient.post('/api/account/login', {
        email,
        password
      });
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to login', error);
      return null;
    }
  }

  async getAccountByEmail(email: string): Promise<AccountDto | null> {
    try {
      const response = await this.apiClient.get(`/api/account/email/${email}`);
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to get account by email', error);
      return null;
    }
  }

  // Player Service Methods
  async createPlayer(username: string, level: number, team: string, friendCode?: string, discordId?: string): Promise<PlayerDto | null> {
    try {
      const response = await this.apiClient.post('/api/player', {
        username,
        level,
        team,
        friendCode,
        discordId
      });
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to create player', error);
      return null;
    }
  }

  async getPlayerById(id: number): Promise<PlayerDto | null> {
    try {
      const response = await this.apiClient.get(`/api/player/${id}`);
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to get player by ID', error);
      return null;
    }
  }

  async getPlayerByUsername(username: string): Promise<PlayerDto | null> {
    try {
      const response = await this.apiClient.get(`/api/player/username/${username}`);
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to get player by username', error);
      return null;
    }
  }

  async getPlayerByDiscordId(discordId: string): Promise<PlayerDto | null> {
    try {
      const response = await this.apiClient.get(`/api/player/discord/${discordId}`);
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to get player by Discord ID', error);
      return null;
    }
  }

  // Location Service Methods
  async createLocation(
    name: string,
    type: string,
    latitude: number,
    longitude: number,
    description?: string,
    address?: string,
    city?: string,
    state?: string,
    country?: string
  ): Promise<LocationDto | null> {
    try {
      const response = await this.apiClient.post('/api/location', {
        name,
        type,
        latitude,
        longitude,
        description,
        address,
        city,
        state,
        country
      });
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to create location', error);
      return null;
    }
  }

  async getLocationById(id: number): Promise<LocationDto | null> {
    try {
      const response = await this.apiClient.get(`/api/location/${id}`);
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to get location by ID', error);
      return null;
    }
  }

  async searchLocationsNearby(latitude: number, longitude: number, radiusKm: number = 10): Promise<LocationDto[]> {
    try {
      const response = await this.apiClient.get('/api/location/search/nearby', {
        latitude,
        longitude,
        radiusKm
      });
      return response.data?.data || [];
    } catch (error) {
      this.logger.error('Failed to search locations nearby', error);
      return [];
    }
  }

  // Gym Service Methods
  async createGym(
    name: string,
    locationId: number,
    level: number,
    team?: string
  ): Promise<GymDto | null> {
    try {
      const response = await this.apiClient.post('/api/gym', {
        name,
        locationId,
        level,
        team
      });
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to create gym', error);
      return null;
    }
  }

  async getGymById(id: number): Promise<GymDto | null> {
    try {
      const response = await this.apiClient.get(`/api/gym/${id}`);
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to get gym by ID', error);
      return null;
    }
  }

  async getGymsByLocation(locationId: number): Promise<GymDto[]> {
    try {
      const response = await this.apiClient.get(`/api/gym/location/${locationId}`);
      return response.data?.data || [];
    } catch (error) {
      this.logger.error('Failed to get gyms by location', error);
      return [];
    }
  }

  async searchGymsNearby(latitude: number, longitude: number, radiusKm: number = 10): Promise<GymDto[]> {
    try {
      const response = await this.apiClient.get('/api/gym/search/nearby', {
        latitude,
        longitude,
        radiusKm
      });
      return response.data?.data || [];
    } catch (error) {
      this.logger.error('Failed to search gyms nearby', error);
      return [];
    }
  }

  // Raid Service Methods
  async createRaid(
    gymId: number,
    pokemonSpecies: string,
    level: number,
    startTime: string,
    endTime: string,
    maxParticipants: number = 20,
    difficulty: string = 'Medium',
    weatherBoost?: string,
    notes?: string
  ): Promise<RaidDto | null> {
    try {
      const response = await this.apiClient.post('/api/raid', {
        gymId,
        pokemonSpecies,
        level,
        startTime,
        endTime,
        maxParticipants,
        difficulty,
        weatherBoost,
        notes
      });
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to create raid', error);
      return null;
    }
  }

  async getRaidById(id: number): Promise<RaidDto | null> {
    try {
      const response = await this.apiClient.get(`/api/raid/${id}`);
      return response.data?.data || null;
    } catch (error) {
      this.logger.error('Failed to get raid by ID', error);
      return null;
    }
  }

  async getRaidsByGym(gymId: number, activeOnly: boolean = true): Promise<RaidDto[]> {
    try {
      const response = await this.apiClient.get(`/api/raid/gym/${gymId}`, {
        activeOnly
      });
      return response.data?.data || [];
    } catch (error) {
      this.logger.error('Failed to get raids by gym', error);
      return [];
    }
  }

  async getActiveRaids(): Promise<RaidDto[]> {
    try {
      const response = await this.apiClient.get('/api/raid/active');
      return response.data?.data || [];
    } catch (error) {
      this.logger.error('Failed to get active raids', error);
      return [];
    }
  }

  async searchRaidsNearby(latitude: number, longitude: number, radiusKm: number = 10): Promise<RaidDto[]> {
    try {
      const response = await this.apiClient.get('/api/raid/search/nearby', {
        latitude,
        longitude,
        radiusKm
      });
      return response.data?.data || [];
    } catch (error) {
      this.logger.error('Failed to search raids nearby', error);
      return [];
    }
  }

  async joinRaid(raidId: number, playerId: number): Promise<boolean> {
    try {
      const response = await this.apiClient.post('/api/raid/join', {
        raidId,
        playerId
      });
      return response.status === 200;
    } catch (error) {
      this.logger.error('Failed to join raid', error);
      return false;
    }
  }

  async leaveRaid(raidId: number, playerId: number): Promise<boolean> {
    try {
      const response = await this.apiClient.post('/api/raid/leave', {
        raidId,
        playerId
      });
      return response.status === 200;
    } catch (error) {
      this.logger.error('Failed to leave raid', error);
      return false;
    }
  }
}
