using AutoMapper;
using FluentValidation;
using RestAPIInvoiceManagement.Application.DTOs;
using RestAPIInvoiceManagement.Domain.Entities;
using RestAPIInvoiceManagement.Domain.Interfaces;

namespace RestAPIInvoiceManagement.Application.Services;

public class PaymentService(IPaymentRepository paymentRepository, IInvoiceRepository invoiceRepository, IMapper mapper, IValidator<CreatePaymentDto> createValidator, IValidator<UpdatePaymentDto> updateValidator)
{
    public async Task<PaymentDto> GetByIdAsync(Guid id)
    {
        var payment = await paymentRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Payment not found");
        return mapper.Map<PaymentDto>(payment);
    }

    public async Task<IEnumerable<PaymentDto>> GetAllByInvoiceIdAsync(Guid invoiceId, int page = 1, int pageSize = 10)
    {
        var payments = await paymentRepository.GetAllByInvoiceIdAsync(invoiceId, page, pageSize);
        return mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<PaymentDto> CreateAsync(Guid invoiceId, CreatePaymentDto dto)
    {
        var validationResult = await createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var invoice = await invoiceRepository.GetByIdAsync(invoiceId) ?? throw new KeyNotFoundException("Invoice not found");
        var payment = new Payment(invoiceId, dto.Amount, dto.PaymentDate, dto.Method);
        invoice.AddPayment(payment);

        await paymentRepository.AddAsync(payment);
        await paymentRepository.SaveChangesAsync();
        return mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto> UpdateAsync(Guid id, UpdatePaymentDto dto)
    {
        var validationResult = await updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var payment = await paymentRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Payment not found");
        payment.Update(dto.Amount, dto.PaymentDate, dto.Method);
        await paymentRepository.SaveChangesAsync();
        return mapper.Map<PaymentDto>(payment);
    }

    public async Task DeleteAsync(Guid id)
    {
        var payment = await paymentRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Payment not found");
        payment.MarkAsDeleted();
        await paymentRepository.SaveChangesAsync();
    }
}