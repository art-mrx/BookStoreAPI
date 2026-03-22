using BookStore.Core.Models;
using BookStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.DataAccess.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(u => u.Email)
            .HasMaxLength(User.MaxEmailLength)
            .IsRequired();

        builder.Property(u => u.FullName)
            .HasMaxLength(User.MaxFullNameLength)
            .IsRequired();
    }
}
