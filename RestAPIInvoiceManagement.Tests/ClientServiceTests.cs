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

public class ClientServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateClientDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateClientDto>> _updateValidatorMock;
    private readonly ClientService _service;

    public ClientServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        _clientRepositoryMock = new Mock<IClientRepository>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateClientDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateClientDto>>();

        _service = new ClientService(
            _clientRepositoryMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidInput_CreatesClient()
    {
        // Arrange
        var createClientDto = new CreateClientDto("Homer Simpson", "hs@example.com", "Evergreen Terrace 742");
        var client = new Client("Homer Simpson", "hs@example.com", "Evergreen Terrace 742");
        var clientDto = new ClientDto(client.Id, "Homer Simpson", "hs@example.com", "Evergreen Terrace 742");

        _createValidatorMock.Setup(v => v.ValidateAsync(createClientDto, default))
            .ReturnsAsync(new ValidationResult());
        _clientRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Client>()))
            .Callback<Client>(c => client = c);
        _mapperMock.Setup(m => m.Map<ClientDto>(It.IsAny<Client>()))
            .Returns(clientDto);

        // Act
        var result = await _service.CreateAsync(createClientDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clientDto, result);
        _clientRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Client>()), Times.Once());
        _clientRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task CreateAsync_InvalidInput_ThrowsValidationException()
    {
        // Arrange
        var createClientDto = new CreateClientDto("", "invalid-email", "Evergreen Terrace 742");
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Name", "Name is required") });
        _createValidatorMock.Setup(v => v.ValidateAsync(createClientDto, default))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(createClientDto));
    }

    [Fact]
    public async Task GetByIdAsync_ExistingClient_ReturnsClientDto()
    {
        // Arrange
        var client = new Client("Homer Simpson", "hs@example.com", "Evergreen Terrace 742");
        var clientDto = new ClientDto(client.Id, "Homer Simpson", "hs@example.com", "Evergreen Terrace 742");

        _clientRepositoryMock.Setup(r => r.GetByIdAsync(client.Id))
            .ReturnsAsync(client);
        _mapperMock.Setup(m => m.Map<ClientDto>(client))
            .Returns(clientDto);

        // Act
        var result = await _service.GetByIdAsync(client.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clientDto, result);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingClient_ThrowsKeyNotFoundException()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        _clientRepositoryMock.Setup(r => r.GetByIdAsync(clientId))
            .ReturnsAsync((Client?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(clientId));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsClientDtos()
    {
        // Arrange
        var clients = new List<Client>
        {
            new Client("Homer Simpson", "hs@example.com", "Evergreen Terrace 742"),
            new Client("Marge Simpson", "ms@example.com", "Evergreen Terrace 742")
        };
        var clientDtos = new List<ClientDto>
        {
            new ClientDto(clients[0].Id, "Homer Simpson", "hs@example.com", "Evergreen Terrace 742"),
            new ClientDto(clients[1].Id, "Marge Simpson", "ms@example.com", "Evergreen Terrace 742")
        };

        _clientRepositoryMock.Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(clients);
        _mapperMock.Setup(m => m.Map<IEnumerable<ClientDto>>(clients))
            .Returns(clientDtos);

        // Act
        var result = await _service.GetAllAsync(1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(clientDtos, result);
    }

    [Fact]
    public async Task UpdateAsync_ValidInput_UpdatesClient()
    {
        // Arrange
        var client = new Client("Homer Simpson", "hs@example.com", "Evergreen Terrace 742");
        var updateClientDto = new UpdateClientDto("Marge Simpson", "ms@example.com", "Evergreen Terrace 742");
        var updatedClientDto = new ClientDto(client.Id, "Marge Simpson", "ms@example.com", "Evergreen Terrace 742");

        _updateValidatorMock.Setup(v => v.ValidateAsync(updateClientDto, default))
            .ReturnsAsync(new ValidationResult());
        _clientRepositoryMock.Setup(r => r.GetByIdAsync(client.Id))
            .ReturnsAsync(client);
        _mapperMock.Setup(m => m.Map<ClientDto>(It.IsAny<Client>()))
            .Returns(updatedClientDto);

        // Act
        var result = await _service.UpdateAsync(client.Id, updateClientDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedClientDto, result);
        _clientRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ExistingClient_MarksAsDeleted()
    {
        // Arrange
        var client = new Client("Homer Simpson", "hs@example.com", "Evergreen Terrace 742");
        _clientRepositoryMock.Setup(r => r.GetByIdAsync(client.Id))
            .ReturnsAsync(client);

        // Act
        await _service.DeleteAsync(client.Id);

        // Assert
        _clientRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }
}