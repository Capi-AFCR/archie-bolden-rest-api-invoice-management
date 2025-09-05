using Microsoft.EntityFrameworkCore;
using RestAPIInvoiceManagement.Domain.Entities;

namespace RestAPIInvoiceManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Number).IsRequired().HasMaxLength(50);
            entity.Property(i => i.IssueDate).IsRequired();
            entity.Property(i => i.DueDate).IsRequired();
            entity.Property(i => i.Amount).IsRequired().HasPrecision(18, 2);
            entity.Property(i => i.TotalAmount).HasPrecision(18, 2);
            entity.Property(i => i.CreatedAt).IsRequired();
            entity.Property(i => i.IsDeleted).IsRequired().HasDefaultValue(false);

            entity.HasOne(i => i.Client)
                  .WithMany(c => c.Invoices)
                  .HasForeignKey(i => i.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(i => i.Payments)
                  .WithOne(p => p.Invoice)
                  .HasForeignKey(p => p.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(255);
            entity.Property(c => c.Address).IsRequired().HasMaxLength(200);
            entity.Property(c => c.CreatedAt).IsRequired();
            entity.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.InvoiceId).IsRequired();
            entity.Property(p => p.Amount).IsRequired().HasPrecision(18, 2);
            entity.Property(p => p.PaymentDate).IsRequired();
            entity.Property(p => p.Method).IsRequired().HasMaxLength(50);
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
           entity.HasKey(u => u.Id);
           entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
           entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
           entity.Property(u => u.CreatedAt).IsRequired();
           entity.Property(u => u.IsDeleted).IsRequired().HasDefaultValue(false);
           entity.HasIndex(u => u.Username).IsUnique();
        });

        modelBuilder.Entity<Invoice>().HasQueryFilter(i => !i.IsDeleted);
        modelBuilder.Entity<Client>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Payment>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
    }
}