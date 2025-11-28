using System;
using SGHSS.Api.DTOs;

namespace SGHSS.Api.Services.Interfaces;

public interface IReceitaService
{
    Task<ReceitaReadDto?> GetByIdAsync(int id);

    Task<ReceitaReadDto> CreateAsync(ReceitaCreateDto dto);

    Task<bool> AddMedicamentoAsync(int receitaId, MedicamentoCreateDto medicamentoDto);

    Task<bool> RemoveMedicamentoAsync(int receitaId, int medicamentoId);

    Task<bool> ValidarCodigoAsync(string codigo);

    Task<IReadOnlyList<ReceitaReadDto>> GetAllByConsultaAsync(int consultaId);
}