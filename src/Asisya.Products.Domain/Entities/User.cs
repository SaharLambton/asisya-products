using Asisya.Products.Domain.Common;

namespace Asisya.Products.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = "User";

    private User() { }

    public User(string username, string email, string passwordHash, string role = "User")
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }
}
