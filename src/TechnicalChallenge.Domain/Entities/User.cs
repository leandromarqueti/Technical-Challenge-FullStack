using TechnicalChallenge.Domain.Common;

namespace TechnicalChallenge.Domain.Entities;

public class User : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    private User() { }

    public User(string name, string email, string passwordHash)
    {
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
    }
}
