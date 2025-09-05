using RestAPIInvoiceManagement.Domain.Entities;

namespace RestAPIInvoiceManagement.Domain.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id);
    Task<IEnumerable<Client>> GetAllAsync(int page = 1, int pageSize = 10);
    Task AddAsync(Client client);
    void Update(Client client);
    void Delete(Client client);
    Task SaveChangesAsync();
}