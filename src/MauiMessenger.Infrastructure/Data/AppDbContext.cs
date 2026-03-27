using MauiMessenger.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MauiMessenger.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public new DbSet<User> Users => Set<User>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ConversationUser> ConversationUsers => Set<ConversationUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(u => u.UserName).HasColumnName("Username").HasMaxLength(100);
            entity.Property(u => u.NormalizedUserName).HasMaxLength(100);
            entity.Property(u => u.DisplayName).HasMaxLength(200);
            entity.Property(u => u.Email).HasMaxLength(255);
            entity.Property(u => u.NormalizedEmail).HasMaxLength(255);
            entity.HasIndex(u => u.NormalizedEmail).IsUnique();
        });

        modelBuilder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.ToTable("AspNetRoles");
        });

        modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("AspNetUserClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("AspNetUserLogins");
        });

        modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("AspNetUserTokens");
        });

        modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("AspNetUserRoles");
        });

        modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("AspNetRoleClaims");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Content).HasMaxLength(4000);
            entity.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);
            entity.HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId);
        });

        modelBuilder.Entity<ConversationUser>(entity =>
        {
            entity.HasKey(cu => new { cu.ConversationId, cu.UserId });
            entity.HasOne(cu => cu.Conversation)
                .WithMany(c => c.ConversationUsers)
                .HasForeignKey(cu => cu.ConversationId);
            entity.HasOne(cu => cu.User)
                .WithMany(u => u.ConversationUsers)
                .HasForeignKey(cu => cu.UserId);
        });
    }
}
