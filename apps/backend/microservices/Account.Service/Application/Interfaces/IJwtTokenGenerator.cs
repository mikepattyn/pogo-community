namespace Account.Service.Application.Interfaces;

/// <summary>
/// Interface for JWT token generation
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for the given user
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="userId">User ID</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(string email, string userId);
}
