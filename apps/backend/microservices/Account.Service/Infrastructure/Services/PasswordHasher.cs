using Account.Service.Application.Interfaces;
using BCrypt.Net;

namespace Account.Service.Infrastructure.Services;

/// <summary>
/// BCrypt implementation of password hasher
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
