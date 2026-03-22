namespace BookStore.API.Contracts;

public record UserResponse(Guid Id, string Email, string FullName);

public record CreateUserRequest(string Email, string FullName);

public record UpdateUserRequest(string Email, string FullName);
