namespace Million.Domain.Entities;

public class User
{
    private User() 
    { 
        Email = string.Empty;
        PasswordHash = string.Empty;
        Role = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public User(Guid id, string email, string passwordHash, string role, 
                string firstName, string lastName, Guid? ownerId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role is required", nameof(role));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        Id = id;
        Email = email.ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
        FirstName = firstName;
        LastName = lastName;
        OwnerId = ownerId;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Guid? OwnerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public static User CreateOwner(string email, string passwordHash, string firstName, string lastName, Guid ownerId)
    {
        return new User(Guid.NewGuid(), email, passwordHash, UserRoles.Owner, firstName, lastName, ownerId);
    }

    public static User CreateClient(string email, string passwordHash, string firstName, string lastName)
    {
        return new User(Guid.NewGuid(), email, passwordHash, UserRoles.Client, firstName, lastName);
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash is required", nameof(newPasswordHash));
        PasswordHash = newPasswordHash;
    }
}

public static class UserRoles
{
    public const string Owner = "Owner";
    public const string Client = "Client";
}
