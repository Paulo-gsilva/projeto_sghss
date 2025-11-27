using System;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Services;

public class LeitoService : ILeitoService
{
    private readonly ApplicationDbContext _context;

    public LeitoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<LeitoReadDto>> GetAllAsync()
    {
        List<Leito> leitos = await _context.Leitos
            .Include(l => l.UnidadeHospitalar)
            .AsNoTracking()
            .ToListAsync();

        List<LeitoReadDto> result = leitos
            .Select(l => new LeitoReadDto(l))
            .ToList();

        return result;
    }

    public async Task<LeitoReadDto?> GetByIdAsync(int id)
    {
        Leito? leito = await _context.Leitos
            .Include(l => l.UnidadeHospitalar)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leito == null)
        {
            return null;
        }

        LeitoReadDto dto = new LeitoReadDto(leito);
        return dto;
    }

    public async Task<LeitoReadDto> CreateAsync(LeitoCreateDto dto)
    {
        Leito leito = new Leito
        {
            Codigo = dto.Codigo,
            Tipo = dto.Tipo,
            UnidadeHospitalarId = dto.UnidadeHospitalarId,
            Status = StatusLeito.Livre
        };

        _context.Leitos.Add(leito);
        await _context.SaveChangesAsync();

        LeitoReadDto result = new LeitoReadDto(leito);
        return result;
    }

    public async Task<bool> UpdateAsync(int id, LeitoCreateDto dto)
    {
        Leito? leito = await _context.Leitos
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leito == null)
        {
            return false;
        }

        leito.Codigo = dto.Codigo;
        leito.Tipo = dto.Tipo;
        leito.UnidadeHospitalarId = dto.UnidadeHospitalarId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AlterarStatusAsync(int id, int status)
    {
        Leito? leito = await _context.Leitos
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leito == null)
        {
            return false;
        }

        if (!System.Enum.IsDefined(typeof(StatusLeito), status))
        {
            throw new System.ArgumentOutOfRangeException(nameof(status), "Status inválido.");
        }

        leito.Status = (StatusLeito)status;
        await _context.SaveChangesAsync();
        return true;
    }
}