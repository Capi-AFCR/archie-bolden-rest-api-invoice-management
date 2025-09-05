using FluentValidation;
using RestAPIInvoiceManagement.Application.DTOs;

namespace RestAPIInvoiceManagement.Application.Validators;

public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentDtoValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty().WithMessage("Invoice ID is required");
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be positive");
        RuleFor(x => x.PaymentDate)
            .NotEmpty().WithMessage("Payment date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Payment date cannot be in the future");
        RuleFor(x => x.Method)
            .NotEmpty().WithMessage("Payment method is required")
            .MaximumLength(50).WithMessage("Payment method cannot exceed 50 characters");
    }
}

public class UpdatePaymentDtoValidator : AbstractValidator<UpdatePaymentDto>
{
    public UpdatePaymentDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be positive");
        RuleFor(x => x.PaymentDate)
            .NotEmpty().WithMessage("Payment date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Payment date cannot be in the future");
        RuleFor(x => x.Method)
            .NotEmpty().WithMessage("Payment method is required")
            .MaximumLength(50).WithMessage("Payment method cannot exceed 50 characters");
    }
}