using BookStore.Core.Models;

namespace BookStore.Core.Abstractions;

public interface IUsersRepository
{
    Task<List<User>> GetAsync(string? search = null, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, string email, string fullName, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, Guid? exceptUserId = null, CancellationToken cancellationToken = default);
}
