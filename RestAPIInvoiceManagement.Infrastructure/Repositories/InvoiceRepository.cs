using Microsoft.EntityFrameworkCore;
using RestAPIInvoiceManagement.Domain.Entities;
using RestAPIInvoiceManagement.Domain.Interfaces;
using RestAPIInvoiceManagement.Infrastructure.Data;

namespace RestAPIInvoiceManagement.Infrastructure.Repositories;

public class InvoiceRepository(AppDbContext context) : IInvoiceRepository
{
    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await context.Invoices
            .Include(i => i.Client)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        return await context.Invoices
            .Include(i => i.Client)
            .Include(i => i.Payments)
            .OrderBy(i => i.IssueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Invoice invoice)
    {
        await context.Invoices.AddAsync(invoice);
    }

    public void Update(Invoice invoice)
    {
        context.Invoices.Update(invoice);
    }

    public void Delete(Invoice invoice)
    {
        context.Invoices.Remove(invoice);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}