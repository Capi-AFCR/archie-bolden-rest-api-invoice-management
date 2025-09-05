using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RestAPIInvoiceManagement.Application.DTOs;
using RestAPIInvoiceManagement.Application.Services;
using RestAPIInvoiceManagement.Infrastructure.Data;

namespace RestAPIInvoiceManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly InvoiceService _invoiceService;

    public InvoicesController(InvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var invoices = await _invoiceService.GetAllAsync(page, pageSize);
        return Ok(invoices);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            return Ok(invoice);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Invoice not found" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
    {
        try
        {
            var created = await _invoiceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceDto dto)
    {
        try
        {
            var updated = await _invoiceService.UpdateAsync(id, dto);
            return Ok(updated);
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _invoiceService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Invoice not found" });
        }
    }

    [HttpGet("{id:guid}/balance")]
    public async Task<IActionResult> GetBalanceDue(Guid id)
    {
        try
        {
            var balance = await _invoiceService.GetBalanceDueAsync(id);
            return Ok(new { BalanceDue = balance });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Invoice not found" });
        }
    }
}