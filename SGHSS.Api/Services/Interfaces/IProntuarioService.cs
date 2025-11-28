using System;
using SGHSS.Api.DTOs;

namespace SGHSS.Api.Services.Interfaces;

public interface IProntuarioService
{
    Task<ProntuarioReadDto?> GetByConsultaIdAsync(int consultaId);

    Task<ProntuarioReadDto?> GetByIdAsync(int id);

    Task<ProntuarioReadDto> CreateOrUpdateByConsultaAsync(ProntuarioCreateDto dto);
    
    Task<bool> DeleteAsync(int id);
}
