using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPIInvoiceManagement.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    private User() { } // For EF Core

    public User(string username, string passwordHash)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
    }

    public void Update(string username, string passwordHash)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}