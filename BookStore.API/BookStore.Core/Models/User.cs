namespace BookStore.Core.Models;

public class User
{
    public const int MaxEmailLength = 256;
    public const int MaxFullNameLength = 200;

    private User(Guid id, string email, string fullName)
    {
        Id = id;
        Email = email;
        FullName = fullName;
    }

    public Guid Id { get; }
    public string Email { get; } = string.Empty;
    public string FullName { get; } = string.Empty;

    public static (User? User, string Error) Create(Guid id, string email, string fullName)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > MaxEmailLength)
            return (null, "Email is required and must be at most 256 characters.");

        if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > MaxFullNameLength)
            return (null, "Full name is required and must be at most 200 characters.");

        return (new User(id, email.Trim().ToLowerInvariant(), fullName.Trim()), string.Empty);
    }

    public static (User? User, string Error) CreateNew(string email, string fullName)
        => Create(Guid.NewGuid(), email, fullName);
}
