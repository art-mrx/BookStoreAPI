using BookStore.Core.Abstractions;
using BookStore.Core.Models;
using BookStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DataAccess.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly BookStoreDbContext _context;

    public BooksRepository(BookStoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<BookEntity> query = _context.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(b =>
                b.Title.Contains(term) ||
                b.Description.Contains(term));
        }

        var entities = await query
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);

        return entities
            .Select(e => Book.Create(e.Id, e.Title, e.Description, e.Price).Book)
            .Where(b => b != null)
            .Cast<Book>()
            .ToList();
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (entity is null)
            return null;

        var (book, _) = Book.Create(entity.Id, entity.Title, entity.Description, entity.Price);
        return book;
    }

    public async Task<Guid> CreateAsync(Book book, CancellationToken cancellationToken = default)
    {
        var entity = new BookEntity
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Price = book.Price
        };

        await _context.Books.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, string title, string description, decimal price, CancellationToken cancellationToken = default)
    {
        var affected = await _context.Books
            .Where(b => b.Id == id)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(b => b.Title, title)
                    .SetProperty(b => b.Description, description)
                    .SetProperty(b => b.Price, price),
                cancellationToken);

        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var affected = await _context.Books
            .Where(b => b.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return affected > 0;
    }
}
