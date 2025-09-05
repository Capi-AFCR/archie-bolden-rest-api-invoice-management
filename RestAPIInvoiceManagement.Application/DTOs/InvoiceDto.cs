using System;
using System.Collections.Generic;

namespace RestAPIInvoiceManagement.Application.DTOs;

public record InvoiceDto(Guid Id, string Number, DateTime IssueDate, DateTime DueDate, Guid ClientId, decimal Amount, decimal TotalAmount, ClientDto? Client, IEnumerable<PaymentDto>? Payments);

public record CreateInvoiceDto(string Number, DateTime IssueDate, DateTime DueDate, Guid ClientId, decimal Amount, List<CreatePaymentDto>? Payments = null);

public record UpdateInvoiceDto(string Number, DateTime IssueDate, DateTime DueDate, Guid ClientId, decimal Amount);