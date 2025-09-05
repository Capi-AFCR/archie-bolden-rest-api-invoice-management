using Microsoft.EntityFrameworkCore;
using RestAPIInvoiceManagement.Domain.Entities;
using RestAPIInvoiceManagement.Domain.Interfaces;
using RestAPIInvoiceManagement.Infrastructure.Data;

namespace RestAPIInvoiceManagement.Infrastructure.Repositories;

public class ClientRepository(AppDbContext context) : IClientRepository
{
    public async Task<Client?> GetByIdAsync(Guid id)
    {
        return await context.Clients
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Client>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        return await context.Clients
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Client client)
    {
        await context.Clients.AddAsync(client);
    }

    public void Update(Client client)
    {
        context.Clients.Update(client);
    }

    public void Delete(Client client)
    {
        context.Clients.Remove(client);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}