namespace BookStore.Core.Models;

public class Book
{
    public const int MaxTitleLength = 250;

    private Book(Guid id, string title, string description, decimal price)
    {
        Id = id;
        Title = title;
        Description = description;
        Price = price;
    }

    public Guid Id { get; }
    public string Title { get; } = string.Empty;
    public string Description { get; } = string.Empty;
    public decimal Price { get; }

    public static (Book? Book, string Error) Create(Guid id, string title, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > MaxTitleLength)
            return (null, "Title cannot be empty or longer than 250 characters.");

        return (new Book(id, title, description, price), string.Empty);
    }

    public static (Book? Book, string Error) CreateNew(string title, string description, decimal price)
        => Create(Guid.NewGuid(), title, description, price);
}
