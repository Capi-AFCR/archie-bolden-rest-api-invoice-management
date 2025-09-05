using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using RestAPIInvoiceManagement.Application.DTOs;
using RestAPIInvoiceManagement.Domain.Entities;
using RestAPIInvoiceManagement.Domain.Interfaces;

namespace RestAPIInvoiceManagement.Application.Services;

public class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<RegisterDto> _registerValidator;

    public AuthenticationService(IUserRepository userRepository, IConfiguration configuration, IValidator<LoginDto> loginValidator, IValidator<RegisterDto> registerValidator)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var validationResult = await _loginValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "default-secret-key");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task RegisterAsync(RegisterDto dto)
    {
        var validationResult = await _registerValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUser != null)
            throw new InvalidOperationException("Username already exists");

        var user = new User(dto.Username, BCrypt.Net.BCrypt.HashPassword(dto.Password));
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}