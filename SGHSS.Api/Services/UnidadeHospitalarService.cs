using System;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Services;

public class UnidadeHospitalarService : IUnidadeHospitalarService
{
    private readonly ApplicationDbContext _context;

    public UnidadeHospitalarService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UnidadeHospitalarReadDto>> GetAllAsync()
    {
        List<UnidadeHospitalar> unidades = await _context.UnidadesHospitalares
            .AsNoTracking()
            .ToListAsync();

        List<UnidadeHospitalarReadDto> result = unidades
            .Select(u => new UnidadeHospitalarReadDto(u))
            .ToList();

        return result;
    }

    public async Task<UnidadeHospitalarReadDto?> GetByIdAsync(int id)
    {
        UnidadeHospitalar? unidade = await _context.UnidadesHospitalares
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (unidade == null)
        {
            return null;
        }

        UnidadeHospitalarReadDto dto = new UnidadeHospitalarReadDto(unidade);
        return dto;
    }

    public async Task<UnidadeHospitalarReadDto> CreateAsync(UnidadeHospitalarCreateDto dto)
    {
        UnidadeHospitalar unidade = new UnidadeHospitalar
        {
            Nome = dto.Nome,
            Endereco = dto.Endereco,
            Tipo = dto.Tipo
        };

        _context.UnidadesHospitalares.Add(unidade);
        await _context.SaveChangesAsync();

        UnidadeHospitalarReadDto result = new UnidadeHospitalarReadDto(unidade);
        return result;
    }

    public async Task<bool> UpdateAsync(int id, UnidadeHospitalarCreateDto dto)
    {
        UnidadeHospitalar? unidade = await _context.UnidadesHospitalares
            .FirstOrDefaultAsync(u => u.Id == id);

        if (unidade == null)
        {
            return false;
        }

        unidade.Nome = dto.Nome;
        unidade.Endereco = dto.Endereco;
        unidade.Tipo = dto.Tipo;

        await _context.SaveChangesAsync();
        return true;
    }
}