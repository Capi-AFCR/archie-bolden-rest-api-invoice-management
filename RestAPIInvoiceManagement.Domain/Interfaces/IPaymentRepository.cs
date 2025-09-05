using RestAPIInvoiceManagement.Domain.Entities;

namespace RestAPIInvoiceManagement.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Payment>> GetAllByInvoiceIdAsync(Guid invoiceId, int page = 1, int pageSize = 10);
    Task AddAsync(Payment payment);
    void Update(Payment payment);
    void Delete(Payment payment);
    Task SaveChangesAsync();
}