using AutoMapper;
using FluentValidation;
using RestAPIInvoiceManagement.Application.DTOs;
using RestAPIInvoiceManagement.Domain.Entities;
using RestAPIInvoiceManagement.Domain.Interfaces;

namespace RestAPIInvoiceManagement.Application.Services;

public class ClientService(IClientRepository clientRepository, IMapper mapper, IValidator<CreateClientDto> createValidator, IValidator<UpdateClientDto> updateValidator)
{
    public async Task<ClientDto> GetByIdAsync(Guid id)
    {
        var client = await clientRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Client not found");
        return mapper.Map<ClientDto>(client);
    }

    public async Task<IEnumerable<ClientDto>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var clients = await clientRepository.GetAllAsync(page, pageSize);
        return mapper.Map<IEnumerable<ClientDto>>(clients);
    }

    public async Task<ClientDto> CreateAsync(CreateClientDto dto)
    {
        var validationResult = await createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var client = mapper.Map<Client>(dto);
        await clientRepository.AddAsync(client);
        await clientRepository.SaveChangesAsync();
        return mapper.Map<ClientDto>(client);
    }

    public async Task<ClientDto> UpdateAsync(Guid id, UpdateClientDto dto)
    {
        var validationResult = await updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var client = await clientRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Client not found");
        client.Update(dto.Name, dto.Email, dto.Address);
        await clientRepository.SaveChangesAsync();
        return mapper.Map<ClientDto>(client);
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await clientRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Client not found");
        client.MarkAsDeleted();
        await clientRepository.SaveChangesAsync();
    }
}