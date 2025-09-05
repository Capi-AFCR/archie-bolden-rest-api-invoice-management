namespace RestAPIInvoiceManagement.Domain.Entities;

public class Invoice : BaseEntity
{
    public string Number { get; private set; } = string.Empty;
    public DateTime IssueDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public Guid ClientId { get; private set; }
    public decimal Amount { get; private set; } // New property for invoice total
    public decimal TotalAmount { get; private set; } // Sum of payments
    public Client? Client { get; private set; }
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    private Invoice() { } // For EF Core

    public Invoice(string number, DateTime issueDate, DateTime dueDate, Guid clientId, decimal amount)
    {
        Number = number ?? throw new ArgumentNullException(nameof(number));
        IssueDate = issueDate;
        DueDate = dueDate;
        ClientId = clientId;
        Amount = amount >= 0 ? amount : throw new ArgumentException("Amount must be non-negative", nameof(amount));
        UpdateTotalAmount();
    }

    public void AddPayment(Payment payment)
    {
        if (payment == null) throw new ArgumentNullException(nameof(payment));
        Payments.Add(payment);
        UpdateTotalAmount();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string number, DateTime issueDate, DateTime dueDate, Guid clientId, decimal amount)
    {
        Number = number ?? throw new ArgumentNullException(nameof(number));
        IssueDate = issueDate;
        DueDate = dueDate;
        ClientId = clientId;
        Amount = amount >= 0 ? amount : throw new ArgumentException("Amount must be non-negative", nameof(amount));
        UpdateTotalAmount();
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private void UpdateTotalAmount()
    {
        TotalAmount = Payments.Where(p => !p.IsDeleted).Sum(p => p.Amount);
    }

    public decimal GetBalanceDue()
    {
        return Amount - TotalAmount;
    }
}