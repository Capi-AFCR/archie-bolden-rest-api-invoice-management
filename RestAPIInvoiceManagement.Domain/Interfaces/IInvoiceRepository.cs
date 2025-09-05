using RestAPIInvoiceManagement.Domain.Entities;

namespace RestAPIInvoiceManagement.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<IEnumerable<Invoice>> GetAllAsync(int page = 1, int pageSize = 10);
    Task AddAsync(Invoice invoice);
    void Update(Invoice invoice);
    void Delete(Invoice invoice);
    Task SaveChangesAsync();
}