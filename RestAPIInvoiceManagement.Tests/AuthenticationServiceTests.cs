using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using FluentValidation;
using FluentValidation.Results;
using RestAPIInvoiceManagement.Application.Services;
using RestAPIInvoiceManagement.Application.DTOs;
using RestAPIInvoiceManagement.Domain.Entities;
using RestAPIInvoiceManagement.Domain.Interfaces;

namespace RestAPIInvoiceManagement.Tests;

public class AuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IValidator<LoginDto>> _loginValidatorMock;
    private readonly Mock<IValidator<RegisterDto>> _registerValidatorMock;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _loginValidatorMock = new Mock<IValidator<LoginDto>>();
        _registerValidatorMock = new Mock<IValidator<RegisterDto>>();

        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("InvoiceManagementAPI");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("InvoiceManagementAPI");
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("dacff57bf14e9128a756c93d8ec27f58");

        _service = new AuthenticationService(
            _userRepositoryMock.Object,
            _configurationMock.Object,
            _loginValidatorMock.Object,
            _registerValidatorMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginDto = new LoginDto("testuser", "Test123!@");
        var user = new User("testuser", BCrypt.Net.BCrypt.HashPassword("Test123!@"));
        _loginValidatorMock.Setup(v => v.ValidateAsync(loginDto, default))
            .ReturnsAsync(new ValidationResult());
        _userRepositoryMock.Setup(r => r.GetByUsernameAsync("testuser"))
            .ReturnsAsync(user);

        // Act
        var token = await _service.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new LoginDto("testuser", "WrongPassword");
        _loginValidatorMock.Setup(v => v.ValidateAsync(loginDto, default))
            .ReturnsAsync(new ValidationResult());
        _userRepositoryMock.Setup(r => r.GetByUsernameAsync("testuser"))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(loginDto));
    }

    [Fact]
    public async Task RegisterAsync_ValidInput_CreatesUser()
    {
        // Arrange
        var registerDto = new RegisterDto("newuser", "New123!@");
        _registerValidatorMock.Setup(v => v.ValidateAsync(registerDto, default))
            .ReturnsAsync(new ValidationResult());
        _userRepositoryMock.Setup(r => r.GetByUsernameAsync("newuser"))
            .ReturnsAsync((User?)null);

        // Act
        await _service.RegisterAsync(registerDto);

        // Assert
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once());
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task RegisterAsync_DuplicateUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        var registerDto = new RegisterDto("existinguser", "New123!@");
        var existingUser = new User("existinguser", "hashedpassword");
        _registerValidatorMock.Setup(v => v.ValidateAsync(registerDto, default))
            .ReturnsAsync(new ValidationResult());
        _userRepositoryMock.Setup(r => r.GetByUsernameAsync("existinguser"))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(registerDto));
    }
}