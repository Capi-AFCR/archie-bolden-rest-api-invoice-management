using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIInvoiceManagement.Application.DTOs;
using RestAPIInvoiceManagement.Application.Services;

namespace RestAPIInvoiceManagement.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")][ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;

    public UsersController(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authenticationService.LoginAsync(dto);
            return Ok(new { Token = token });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { Message = "Invalid username or password" });
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            await _authenticationService.RegisterAsync(dto);
            return Ok(new { Message = "User registered successfully" });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
    }
}