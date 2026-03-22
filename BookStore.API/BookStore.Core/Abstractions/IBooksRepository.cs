using BookStore.Core.Models;

namespace BookStore.Core.Abstractions;

public interface IBooksRepository
{
    Task<List<Book>> GetAsync(string? search = null, CancellationToken cancellationToken = default);

    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(Book book, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, string title, string description, decimal price, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
