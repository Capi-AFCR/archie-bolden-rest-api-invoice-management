using AutoMapper;
using FluentValidation;
using RestAPIInvoiceManagement.Application.DTOs;
using RestAPIInvoiceManagement.Domain.Entities;
using RestAPIInvoiceManagement.Domain.Interfaces;

namespace RestAPIInvoiceManagement.Application.Services;

public class InvoiceService(IInvoiceRepository invoiceRepository, IClientRepository clientRepository, IPaymentRepository paymentRepository, IMapper mapper, IValidator<CreateInvoiceDto> createValidator, IValidator<UpdateInvoiceDto> updateValidator)
{
    public async Task<InvoiceDto> GetByIdAsync(Guid id)
    {
        var invoice = await invoiceRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Invoice not found");
        return mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var invoices = await invoiceRepository.GetAllAsync(page, pageSize);
        return mapper.Map<IEnumerable<InvoiceDto>>(invoices);
    }

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto)
    {
        var validationResult = await createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var client = await clientRepository.GetByIdAsync(dto.ClientId) ?? throw new KeyNotFoundException("Client not found");

        var invoice = new Invoice(dto.Number, dto.IssueDate, dto.DueDate, dto.ClientId, dto.Amount);
        if (dto.Payments != null)
        {
            foreach (var paymentDto in dto.Payments)
            {
                var payment = new Payment(invoice.Id, paymentDto.Amount, paymentDto.PaymentDate, paymentDto.Method);
                await paymentRepository.AddAsync(payment);
                invoice.AddPayment(payment);
            }
        }

        await invoiceRepository.AddAsync(invoice);
        await invoiceRepository.SaveChangesAsync();
        return mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<InvoiceDto> UpdateAsync(Guid id, UpdateInvoiceDto dto)
    {
        var validationResult = await updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var invoice = await invoiceRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Invoice not found");
        var client = await clientRepository.GetByIdAsync(dto.ClientId) ?? throw new KeyNotFoundException("Client not found");

        invoice.Update(dto.Number, dto.IssueDate, dto.DueDate, dto.ClientId, dto.Amount);
        await invoiceRepository.SaveChangesAsync();
        return mapper.Map<InvoiceDto>(invoice);
    }

    public async Task DeleteAsync(Guid id)
    {
        var invoice = await invoiceRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Invoice not found");
        invoice.MarkAsDeleted();
        await invoiceRepository.SaveChangesAsync();
    }

    public async Task<decimal> GetBalanceDueAsync(Guid id)
    {
        var invoice = await invoiceRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Invoice not found");
        return invoice.GetBalanceDue();
    }
}