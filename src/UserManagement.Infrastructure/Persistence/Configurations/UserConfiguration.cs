using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;
using UserManagement.Domain.ValueObjects;

namespace UserManagement.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property<string>("_name")
            .HasColumnName("Name")
            .IsRequired()
            .HasMaxLength(Name.MaxLength);

        builder.Property<string>("_email")
            .HasColumnName("Email")
            .IsRequired()
            .HasMaxLength(Email.MaxLength);

        builder.HasIndex("_email")
            .IsUnique();

        builder.Property<string>("_passwordHash")
            .HasColumnName("PasswordHash")
            .IsRequired()
            .HasMaxLength(Password.MaxLength);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .IsRequired(false);
    }
}
