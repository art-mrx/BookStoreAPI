using BookStore.Core.Abstractions;
using BookStore.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(BookStoreDbContext));
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string '{nameof(BookStoreDbContext)}' is not configured.");

        services.AddDbContext<BookStoreDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IBooksRepository, BooksRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();

        return services;
    }
}
