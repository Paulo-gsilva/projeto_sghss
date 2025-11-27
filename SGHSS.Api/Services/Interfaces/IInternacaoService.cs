using System;
using SGHSS.Api.DTOs;

namespace SGHSS.Api.Services.Interfaces;

public interface IInternacaoService
{
    Task<IReadOnlyList<InternacaoReadDto>> GetAllAsync();

    Task<InternacaoReadDto?> GetByIdAsync(int id);

    Task<InternacaoReadDto> InternarAsync(InternacaoCreateDto dto);

    Task<bool> TransferirAsync(int internacaoId, int novoLeitoId);

    Task<bool> DarAltaAsync(int internacaoId);
}
