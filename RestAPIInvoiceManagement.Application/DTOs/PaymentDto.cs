namespace RestAPIInvoiceManagement.Application.DTOs;

public record PaymentDto(Guid Id, Guid InvoiceId, decimal Amount, DateTime PaymentDate, string Method);

public record CreatePaymentDto(Guid InvoiceId, decimal Amount, DateTime PaymentDate, string Method);

public record UpdatePaymentDto(decimal Amount, DateTime PaymentDate, string Method);