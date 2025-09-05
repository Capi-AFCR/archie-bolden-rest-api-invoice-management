namespace RestAPIInvoiceManagement.Application.DTOs;

public record ClientDto(Guid Id, string Name, string Email, string Address);

public record CreateClientDto(string Name, string Email, string Address);

public record UpdateClientDto(string Name, string Email, string Address);