using System;
using SGHSS.Api.DTOs;

namespace SGHSS.Api.Services.Interfaces;

public interface IPacienteService
{
    Task<IReadOnlyList<PacienteReadDto>> GetAllAsync();
    Task<PacienteReadDto?> GetByIdAsync(int id);
    Task<PacienteReadDto> CreateAsync(PacienteCreateDto dto);
    Task<bool> UpdateAsync(int id, PacienteCreateDto dto);
    Task<bool> InativarAsync(int id);
}