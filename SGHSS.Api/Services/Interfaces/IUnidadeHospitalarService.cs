using System;
using SGHSS.Api.DTOs;

namespace SGHSS.Api.Services.Interfaces;

public interface IUnidadeHospitalarService
{
    Task<IReadOnlyList<UnidadeHospitalarReadDto>> GetAllAsync();

    Task<UnidadeHospitalarReadDto?> GetByIdAsync(int id);

    Task<UnidadeHospitalarReadDto> CreateAsync(UnidadeHospitalarCreateDto dto);
    
    Task<bool> UpdateAsync(int id, UnidadeHospitalarCreateDto dto);
}