using Microsoft.EntityFrameworkCore;
using RestAPIInvoiceManagement.Domain.Entities;
using RestAPIInvoiceManagement.Domain.Interfaces;
using RestAPIInvoiceManagement.Infrastructure.Data;

namespace RestAPIInvoiceManagement.Infrastructure.Repositories;

public class PaymentRepository(AppDbContext context) : IPaymentRepository
{
    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await context.Payments
            .Include(p => p.Invoice)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Payment>> GetAllByInvoiceIdAsync(Guid invoiceId, int page = 1, int pageSize = 10)
    {
        return await context.Payments
            .Where(p => p.InvoiceId == invoiceId)
            .Include(p => p.Invoice)
            .OrderBy(p => p.PaymentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Payment payment)
    {
        await context.Payments.AddAsync(payment);
    }

    public void Update(Payment payment)
    {
        context.Payments.Update(payment);
    }

    public void Delete(Payment payment)
    {
        context.Payments.Remove(payment);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}