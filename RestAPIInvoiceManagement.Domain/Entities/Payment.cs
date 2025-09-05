namespace RestAPIInvoiceManagement.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string Method { get; private set; } = string.Empty; // e.g., CreditCard, BankTransfer
    public Invoice? Invoice { get; private set; } // Navigation property

    private Payment() { } // For EF Core

    public Payment(Guid invoiceId, decimal amount, DateTime paymentDate, string method)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
        InvoiceId = invoiceId;
        Amount = amount;
        PaymentDate = paymentDate;
        Method = method ?? throw new ArgumentNullException(nameof(method));
    }

    public void Update(decimal amount, DateTime paymentDate, string method)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
        Amount = amount;
        PaymentDate = paymentDate;
        Method = method ?? throw new ArgumentNullException(nameof(method));
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}