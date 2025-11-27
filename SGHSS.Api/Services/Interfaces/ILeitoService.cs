using System;
using SGHSS.Api.DTOs;

namespace SGHSS.Api.Services.Interfaces;

public interface ILeitoService
{
    Task<IReadOnlyList<LeitoReadDto>> GetAllAsync();

    Task<LeitoReadDto?> GetByIdAsync(int id);

    Task<LeitoReadDto> CreateAsync(LeitoCreateDto dto);

    Task<bool> UpdateAsync(int id, LeitoCreateDto dto);
    
    Task<bool> AlterarStatusAsync(int id, int status);
}