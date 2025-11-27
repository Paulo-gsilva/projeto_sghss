using System;
using SGHSS.Api.DTOs;

namespace SGHSS.Api.Services.Interfaces;

public interface IProfissionalSaudeService
{
    Task<IReadOnlyList<ProfissionalSaudeReadDto>> GetAllAsync();

    Task<ProfissionalSaudeReadDto?> GetByIdAsync(int id);

    Task<ProfissionalSaudeReadDto> CreateAsync(ProfissionalSaudeCreateDto dto);

    Task<bool> UpdateAsync(int id, ProfissionalSaudeCreateDto dto);
    
    Task<bool> InativarAsync(int id);
}