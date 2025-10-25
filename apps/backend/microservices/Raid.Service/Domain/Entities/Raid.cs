using Pogo.Shared.Kernel;

namespace Raid.Service.Domain.Entities;

/// <summary>
/// Raid entity representing a Pokemon Raid
/// </summary>
public class Raid : BaseEntity
{
    /// <summary>
    /// Gym ID where the raid is taking place
    /// </summary>
    public int GymId { get; set; }

    /// <summary>
    /// Pokemon species for the raid
    /// </summary>
    public string PokemonSpecies { get; set; } = string.Empty;

    /// <summary>
    /// Raid level (1-5, where 5 is legendary)
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Raid start time
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Raid end time
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Whether the raid is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the raid has been completed
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Whether the raid has been cancelled
    /// </summary>
    public bool IsCancelled { get; set; } = false;

    /// <summary>
    /// Maximum number of participants allowed
    /// </summary>
    public int MaxParticipants { get; set; } = 20;

    /// <summary>
    /// Current number of participants
    /// </summary>
    public int CurrentParticipants { get; set; } = 0;

    /// <summary>
    /// Raid difficulty (Easy, Medium, Hard, Legendary)
    /// </summary>
    public string Difficulty { get; set; } = "Medium";

    /// <summary>
    /// Weather boost type (if any)
    /// </summary>
    public string? WeatherBoost { get; set; }

    /// <summary>
    /// Additional notes about the raid
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Checks if the raid is currently happening
    /// </summary>
    public bool IsCurrentlyActive => IsActive && !IsCompleted && !IsCancelled && 
                                   DateTime.UtcNow >= StartTime && DateTime.UtcNow <= EndTime;

    /// <summary>
    /// Checks if the raid has ended
    /// </summary>
    public bool HasEnded => DateTime.UtcNow > EndTime;

    /// <summary>
    /// Checks if the raid is starting soon (within 15 minutes)
    /// </summary>
    public bool IsStartingSoon => !HasEnded && (StartTime - DateTime.UtcNow).TotalMinutes <= 15;

    /// <summary>
    /// Gets the time remaining until the raid starts
    /// </summary>
    public TimeSpan TimeUntilStart => StartTime - DateTime.UtcNow;

    /// <summary>
    /// Gets the time remaining until the raid ends
    /// </summary>
    public TimeSpan TimeUntilEnd => EndTime - DateTime.UtcNow;

    /// <summary>
    /// Gets the duration of the raid
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Starts the raid
    /// </summary>
    public void Start()
    {
        if (DateTime.UtcNow < StartTime)
        {
            throw new InvalidOperationException("Cannot start raid before scheduled start time");
        }

        IsActive = true;
        Touch();
    }

    /// <summary>
    /// Completes the raid
    /// </summary>
    public void Complete()
    {
        IsCompleted = true;
        IsActive = false;
        Touch();
    }

    /// <summary>
    /// Cancels the raid
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        IsActive = false;
        Touch();
    }

    /// <summary>
    /// Adds a participant to the raid
    /// </summary>
    public void AddParticipant()
    {
        if (CurrentParticipants >= MaxParticipants)
        {
            throw new InvalidOperationException("Raid is full");
        }

        if (HasEnded)
        {
            throw new InvalidOperationException("Cannot join completed raid");
        }

        CurrentParticipants++;
        Touch();
    }

    /// <summary>
    /// Removes a participant from the raid
    /// </summary>
    public void RemoveParticipant()
    {
        if (CurrentParticipants <= 0)
        {
            throw new InvalidOperationException("No participants to remove");
        }

        CurrentParticipants--;
        Touch();
    }

    /// <summary>
    /// Updates the raid times
    /// </summary>
    /// <param name="startTime">New start time</param>
    /// <param name="endTime">New end time</param>
    public void UpdateTimes(DateTime startTime, DateTime endTime)
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be before end time");
        }

        StartTime = startTime;
        EndTime = endTime;
        Touch();
    }

    /// <summary>
    /// Updates the Pokemon species
    /// </summary>
    /// <param name="pokemonSpecies">New Pokemon species</param>
    public void UpdatePokemon(string pokemonSpecies)
    {
        if (string.IsNullOrWhiteSpace(pokemonSpecies))
        {
            throw new ArgumentException("Pokemon species cannot be empty", nameof(pokemonSpecies));
        }

        PokemonSpecies = pokemonSpecies;
        Touch();
    }
}
