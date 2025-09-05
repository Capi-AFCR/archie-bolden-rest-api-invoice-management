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

public class InvoiceServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateInvoiceDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateInvoiceDto>> _updateValidatorMock;
    private readonly InvoiceService _service;

    public InvoiceServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateInvoiceDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateInvoiceDto>>();

        _service = new InvoiceService(
            _invoiceRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidInput_CreatesInvoice()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var createInvoiceDto = new CreateInvoiceDto(
            Number: "INV-001",
            IssueDate: DateTime.UtcNow,
            DueDate: DateTime.UtcNow.AddDays(30),
            ClientId: clientId,
            Amount: 500.00m,
            Payments: null);
        var client = new Client("John Doe", "john@example.com", "123 Main St");
        var invoice = new Invoice("INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 500.00m);
        var invoiceDto = new InvoiceDto(invoice.Id, "INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 500.00m, 0.00m, null, null);

        _createValidatorMock.Setup(v => v.ValidateAsync(createInvoiceDto, default))
            .ReturnsAsync(new ValidationResult());
        _clientRepositoryMock.Setup(r => r.GetByIdAsync(clientId))
            .ReturnsAsync(client);
        _invoiceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Invoice>()))
            .Callback<Invoice>(i => invoice = i);
        _mapperMock.Setup(m => m.Map<InvoiceDto>(It.IsAny<Invoice>()))
            .Returns(invoiceDto);

        // Act
        var result = await _service.CreateAsync(createInvoiceDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoiceDto, result);
        _invoiceRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Invoice>()), Times.Once());
        _invoiceRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task CreateAsync_NonExistingClient_ThrowsKeyNotFoundException()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var createInvoiceDto = new CreateInvoiceDto(
            Number: "INV-001",
            IssueDate: DateTime.UtcNow,
            DueDate: DateTime.UtcNow.AddDays(30),
            ClientId: clientId,
            Amount: 500.00m,
            Payments: null);
        _createValidatorMock.Setup(v => v.ValidateAsync(createInvoiceDto, default))
            .ReturnsAsync(new ValidationResult());
        _clientRepositoryMock.Setup(r => r.GetByIdAsync(clientId))
            .ReturnsAsync((Client?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(createInvoiceDto));
    }

    [Fact]
    public async Task GetByIdAsync_ExistingInvoice_ReturnsInvoiceDto()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var invoice = new Invoice("INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 500.00m);
        var invoiceDto = new InvoiceDto(invoice.Id, "INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 500.00m, 0.00m, null, null);

        _invoiceRepositoryMock.Setup(r => r.GetByIdAsync(invoice.Id))
            .ReturnsAsync(invoice);
        _mapperMock.Setup(m => m.Map<InvoiceDto>(invoice))
            .Returns(invoiceDto);

        // Act
        var result = await _service.GetByIdAsync(invoice.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoiceDto, result);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingInvoice_ThrowsKeyNotFoundException()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        _invoiceRepositoryMock.Setup(r => r.GetByIdAsync(invoiceId))
            .ReturnsAsync((Invoice?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(invoiceId));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsInvoiceDtos()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var invoices = new List<Invoice>
        {
            new Invoice("INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 500.00m),
            new Invoice("INV-002", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 600.00m)
        };
        var invoiceDtos = new List<InvoiceDto>
        {
            new InvoiceDto(invoices[0].Id, "INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 500.00m, 0.00m, null, null),
            new InvoiceDto(invoices[1].Id, "INV-002", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 600.00m, 0.00m, null, null)
        };

        _invoiceRepositoryMock.Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(invoices);
        _mapperMock.Setup(m => m.Map<IEnumerable<InvoiceDto>>(invoices))
            .Returns(invoiceDtos);

        // Act
        var result = await _service.GetAllAsync(1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(invoiceDtos, result);
    }

    [Fact]
    public async Task UpdateAsync_ValidInput_UpdatesInvoice()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var invoice = new Invoice("INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 500.00m);
        var updateInvoiceDto = new UpdateInvoiceDto("INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 600.00m);
        var client = new Client("Homer Simpson", "hs@example.com", "Evergreen Terrace 742");
        var updatedInvoiceDto = new InvoiceDto(invoice.Id, "INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 600.00m, 0.00m, null, null);

        _updateValidatorMock.Setup(v => v.ValidateAsync(updateInvoiceDto, default))
            .ReturnsAsync(new ValidationResult());
        _clientRepositoryMock.Setup(r => r.GetByIdAsync(clientId))
            .ReturnsAsync(client);
        _invoiceRepositoryMock.Setup(r => r.GetByIdAsync(invoice.Id))
            .ReturnsAsync(invoice);
        _mapperMock.Setup(m => m.Map<InvoiceDto>(It.IsAny<Invoice>()))
            .Returns(updatedInvoiceDto);

        // Act
        var result = await _service.UpdateAsync(invoice.Id, updateInvoiceDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedInvoiceDto, result);
        _invoiceRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ExistingInvoice_MarksAsDeleted()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var invoice = new Invoice("INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 500.00m);
        _invoiceRepositoryMock.Setup(r => r.GetByIdAsync(invoice.Id))
            .ReturnsAsync(invoice);

        // Act
        await _service.DeleteAsync(invoice.Id);

        // Assert
        _invoiceRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task GetBalanceDueAsync_ExistingInvoice_ReturnsBalance()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var invoice = new Invoice("INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), clientId, 500.00m);
        var payment = new Payment(invoice.Id, 100.00m, DateTime.UtcNow, "CreditCard");
        invoice.AddPayment(payment);
        _invoiceRepositoryMock.Setup(r => r.GetByIdAsync(invoice.Id))
            .ReturnsAsync(invoice);

        // Act
        var balance = await _service.GetBalanceDueAsync(invoice.Id);

        // Assert
        Assert.Equal(400.00m, balance);
    }
}