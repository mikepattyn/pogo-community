using Pogo.Shared.Kernel;

namespace Account.Service.Domain.Entities;

/// <summary>
/// Account entity representing user authentication and account information
/// </summary>
public class Account : BaseEntity
{
    /// <summary>
    /// Player ID associated with this account
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// Hashed password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Date when the account was created
    /// </summary>
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Number of wrong password attempts
    /// </summary>
    public int WrongAttempts { get; set; } = 0;

    /// <summary>
    /// Date when the account was locked out (null if not locked)
    /// </summary>
    public DateTime? LockedOut { get; set; }

    /// <summary>
    /// Email address for the account
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Checks if the account is currently locked
    /// </summary>
    public bool IsLocked => LockedOut.HasValue && LockedOut.Value > DateTime.UtcNow;

    /// <summary>
    /// Increments the wrong attempts counter
    /// </summary>
    public void IncrementWrongAttempts()
    {
        WrongAttempts++;
        if (WrongAttempts >= 5)
        {
            LockedOut = DateTime.UtcNow.AddMinutes(30); // Lock for 30 minutes
        }
        Touch();
    }

    /// <summary>
    /// Resets the wrong attempts counter
    /// </summary>
    public void ResetWrongAttempts()
    {
        WrongAttempts = 0;
        LockedOut = null;
        Touch();
    }
}
