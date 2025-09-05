using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIInvoiceManagement.Application.DTOs;
using RestAPIInvoiceManagement.Application.Services;

namespace RestAPIInvoiceManagement.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")][Authorize][ApiVersion("1.0")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var payment = await _paymentService.GetByIdAsync(id);
            return Ok(payment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Payment not found" });
        }
    }

    [HttpGet("by-invoice/{invoiceId:guid}")]
    public async Task<IActionResult> GetAllByInvoiceId(Guid invoiceId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var payments = await _paymentService.GetAllByInvoiceIdAsync(invoiceId, page, pageSize);
        return Ok(payments);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
    {
        try
        {
            var created = await _paymentService.CreateAsync(dto.InvoiceId, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Invoice not found" });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentDto dto)
    {
        try
        {
            var updated = await _paymentService.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Payment not found" });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _paymentService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Payment not found" });
        }
    }
}