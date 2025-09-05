using FluentValidation;
using RestAPIInvoiceManagement.Application.DTOs;

namespace RestAPIInvoiceManagement.Application.Validators;

public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceDtoValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Invoice number is required")
            .MaximumLength(50).WithMessage("Invoice number cannot exceed 50 characters");
        RuleFor(x => x.IssueDate)
            .NotEmpty().WithMessage("Issue date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Issue date cannot be in the future");
        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required")
            .GreaterThanOrEqualTo(x => x.IssueDate).WithMessage("Due date must be on or after issue date");
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client ID is required");
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Amount must be non-negative");
        RuleForEach(x => x.Payments)
            .SetValidator(new CreatePaymentDtoValidator())
            .When(x => x.Payments != null && x.Payments.Any());
    }
}

public class UpdateInvoiceDtoValidator : AbstractValidator<UpdateInvoiceDto>
{
    public UpdateInvoiceDtoValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Invoice number is required")
            .MaximumLength(50).WithMessage("Invoice number cannot exceed 50 characters");
        RuleFor(x => x.IssueDate)
            .NotEmpty().WithMessage("Issue date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Issue date cannot be in the future");
        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required")
            .GreaterThanOrEqualTo(x => x.IssueDate).WithMessage("Due date must be on or after issue date");
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client ID is required");
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Amount must be non-negative");
    }
}