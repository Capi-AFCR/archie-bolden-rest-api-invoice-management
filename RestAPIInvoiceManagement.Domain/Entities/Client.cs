namespace RestAPIInvoiceManagement.Domain.Entities;

public class Client : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public ICollection<Invoice> Invoices { get; private set; } = new List<Invoice>();

    private Client() { } // For EF Core

    public Client(string name, string email, string address)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public void Update(string name, string email, string address)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}