using System;
using SGHSS.Api.DTOs;

namespace SGHSS.Api.Services.Interfaces;

public interface IConsultaService
{
    Task<IReadOnlyList<ConsultaReadDto>> GetAllAsync();

    Task<ConsultaReadDto?> GetByIdAsync(int id);

    Task<ConsultaReadDto> AgendarAsync(ConsultaCreateDto dto);

    Task<bool> CancelarAsync(int id);
    
    Task<bool> ConcluirAsync(int id);
}