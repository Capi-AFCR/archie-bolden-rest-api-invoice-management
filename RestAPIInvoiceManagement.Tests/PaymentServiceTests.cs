using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentValidation;
using FluentValidation.Results;
using AutoMapper;
using RestAPIInvoiceManagement.Application.Services;
using RestAPIInvoiceManagement.Application.DTOs;
using RestAPIInvoiceManagement.Domain.Entities;
using RestAPIInvoiceManagement.Domain.Interfaces;
using RestAPIInvoiceManagement.Infrastructure.Data;

namespace RestAPIInvoiceManagement.Tests;

public class PaymentServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreatePaymentDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdatePaymentDto>> _updateValidatorMock;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreatePaymentDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdatePaymentDto>>();

        _service = new PaymentService(
            _paymentRepositoryMock.Object,
            _invoiceRepositoryMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidInput_CreatesPayment()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var createPaymentDto = new CreatePaymentDto(invoiceId, 100.00m, DateTime.UtcNow, "CreditCard");
        var invoice = new Invoice("INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), Guid.NewGuid(), 500.00m);
        var payment = new Payment(invoiceId, 100.00m, DateTime.UtcNow, "CreditCard");
        var paymentDto = new PaymentDto(payment.Id, invoiceId, 100.00m, DateTime.UtcNow, "CreditCard");

        _createValidatorMock.Setup(v => v.ValidateAsync(createPaymentDto, default))
            .ReturnsAsync(new ValidationResult());
        _invoiceRepositoryMock.Setup(r => r.GetByIdAsync(invoiceId))
            .ReturnsAsync(invoice);
        _paymentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Payment>()))
            .Callback<Payment>(p => payment = p);
        _mapperMock.Setup(m => m.Map<PaymentDto>(It.IsAny<Payment>()))
            .Returns(paymentDto);

        // Act
        var result = await _service.CreateAsync(invoiceId, createPaymentDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(paymentDto, result);
        _paymentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Once());
        _paymentRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task CreateAsync_NonExistingInvoice_ThrowsKeyNotFoundException()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var createPaymentDto = new CreatePaymentDto(invoiceId, 100.00m, DateTime.UtcNow, "CreditCard");
        _createValidatorMock.Setup(v => v.ValidateAsync(createPaymentDto, default))
            .ReturnsAsync(new ValidationResult());
        _invoiceRepositoryMock.Setup(r => r.GetByIdAsync(invoiceId))
            .ReturnsAsync((Invoice?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(invoiceId, createPaymentDto));
    }

    [Fact]
    public async Task GetByIdAsync_ExistingPayment_ReturnsPaymentDto()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var payment = new Payment(invoiceId, 100.00m, DateTime.UtcNow, "CreditCard");
        var paymentDto = new PaymentDto(payment.Id, invoiceId, 100.00m, DateTime.UtcNow, "CreditCard");

        _paymentRepositoryMock.Setup(r => r.GetByIdAsync(payment.Id))
            .ReturnsAsync(payment);
        _mapperMock.Setup(m => m.Map<PaymentDto>(payment))
            .Returns(paymentDto);

        // Act
        var result = await _service.GetByIdAsync(payment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(paymentDto, result);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingPayment_ThrowsKeyNotFoundException()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        _paymentRepositoryMock.Setup(r => r.GetByIdAsync(paymentId))
            .ReturnsAsync((Payment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(paymentId));
    }

    [Fact]
    public async Task GetAllByInvoiceIdAsync_ReturnsPaymentDtos()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var payments = new List<Payment>
        {
            new Payment(invoiceId, 100.00m, DateTime.UtcNow, "CreditCard"),
            new Payment(invoiceId, 200.00m, DateTime.UtcNow, "BankTransfer")
        };
        var paymentDtos = new List<PaymentDto>
        {
            new PaymentDto(payments[0].Id, invoiceId, 100.00m, DateTime.UtcNow, "CreditCard"),
            new PaymentDto(payments[1].Id, invoiceId, 200.00m, DateTime.UtcNow, "BankTransfer")
        };

        _paymentRepositoryMock.Setup(r => r.GetAllByInvoiceIdAsync(invoiceId, 1, 10))
            .ReturnsAsync(payments);
        _mapperMock.Setup(m => m.Map<IEnumerable<PaymentDto>>(payments))
            .Returns(paymentDtos);

        // Act
        var result = await _service.GetAllByInvoiceIdAsync(invoiceId, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(paymentDtos, result);
    }

    [Fact]
    public async Task UpdateAsync_ValidInput_UpdatesPayment()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var payment = new Payment(invoiceId, 100.00m, DateTime.UtcNow, "CreditCard");
        var updatePaymentDto = new UpdatePaymentDto(150.00m, DateTime.UtcNow, "BankTransfer");
        var updatedPaymentDto = new PaymentDto(payment.Id, invoiceId, 150.00m, DateTime.UtcNow, "BankTransfer");

        _updateValidatorMock.Setup(v => v.ValidateAsync(updatePaymentDto, default))
            .ReturnsAsync(new ValidationResult());
        _paymentRepositoryMock.Setup(r => r.GetByIdAsync(payment.Id))
            .ReturnsAsync(payment);
        _mapperMock.Setup(m => m.Map<PaymentDto>(It.IsAny<Payment>()))
            .Returns(updatedPaymentDto);

        // Act
        var result = await _service.UpdateAsync(payment.Id, updatePaymentDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedPaymentDto, result);
        _paymentRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ExistingPayment_MarksAsDeleted()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var payment = new Payment(invoiceId, 100.00m, DateTime.UtcNow, "CreditCard");
        _paymentRepositoryMock.Setup(r => r.GetByIdAsync(payment.Id))
            .ReturnsAsync(payment);

        // Act
        await _service.DeleteAsync(payment.Id);

        // Assert
        _paymentRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }
}