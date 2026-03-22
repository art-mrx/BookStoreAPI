using BookStore.Core.Abstractions;
using BookStore.Core.Models;
using BookStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly BookStoreDbContext _context;

    public UsersRepository(BookStoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<UserEntity> query = _context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(u =>
                u.Email.Contains(term) ||
                u.FullName.ToLower().Contains(term));
        }

        var entities = await query
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);

        return entities
            .Select(e => User.Create(e.Id, e.Email, e.FullName).User)
            .Where(u => u != null)
            .Cast<User>()
            .ToList();
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (entity is null)
            return null;

        var (user, _) = User.Create(entity.Id, entity.Email, entity.FullName);
        return user;
    }

    public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = new UserEntity
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };

        await _context.Users.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, string email, string fullName, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var affected = await _context.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(u => u.Email, normalized)
                    .SetProperty(u => u.FullName, fullName.Trim()),
                cancellationToken);

        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var affected = await _context.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return affected > 0;
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? exceptUserId = null, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();

        var query = _context.Users.AsNoTracking().Where(u => u.Email == normalized);

        if (exceptUserId.HasValue)
            query = query.Where(u => u.Id != exceptUserId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}
