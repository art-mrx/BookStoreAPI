namespace BookStore.API.Contracts;

public record BookResponse(Guid Id, string Title, string Description, decimal Price);

public record CreateBookRequest(string Title, string Description, decimal Price);

public record UpdateBookRequest(string Title, string Description, decimal Price);
