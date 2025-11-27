using System;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Services;

public class ProfissionalSaudeService : IProfissionalSaudeService
{
    private readonly ApplicationDbContext _context;

    public ProfissionalSaudeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProfissionalSaudeReadDto>> GetAllAsync()
    {
        List<ProfissionalSaude> profissionais = await _context.ProfissionaisSaude
            .Include(p => p.UnidadeHospitalar)
            .AsNoTracking()
            .ToListAsync();

        List<ProfissionalSaudeReadDto> result = profissionais
            .Select(p => new ProfissionalSaudeReadDto(p))
            .ToList();

        return result;
    }

    public async Task<ProfissionalSaudeReadDto?> GetByIdAsync(int id)
    {
        ProfissionalSaude? profissional = await _context.ProfissionaisSaude
            .Include(p => p.UnidadeHospitalar)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profissional == null)
        {
            return null;
        }

        ProfissionalSaudeReadDto dto = new ProfissionalSaudeReadDto(profissional);
        return dto;
    }

    public async Task<ProfissionalSaudeReadDto> CreateAsync(ProfissionalSaudeCreateDto dto)
    {
        bool cpfExists = await _context.ProfissionaisSaude
            .AnyAsync(p => p.Cpf == dto.Cpf);

        if (cpfExists)
        {
            throw new System.InvalidOperationException("CPF de profissional já cadastrado.");
        }

        ProfissionalSaude profissional = new ProfissionalSaude
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            RegistroProfissional = dto.RegistroProfissional,
            Especialidade = dto.Especialidade,
            Email = dto.Email,
            Telefone = dto.Telefone,
            UnidadeHospitalarId = dto.UnidadeHospitalarId,
            Ativo = true
        };

        _context.ProfissionaisSaude.Add(profissional);
        await _context.SaveChangesAsync();

        ProfissionalSaudeReadDto result = new ProfissionalSaudeReadDto(profissional);
        return result;
    }

    public async Task<bool> UpdateAsync(int id, ProfissionalSaudeCreateDto dto)
    {
        ProfissionalSaude? profissional = await _context.ProfissionaisSaude
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profissional == null)
        {
            return false;
        }

        profissional.Nome = dto.Nome;
        profissional.Email = dto.Email;
        profissional.Telefone = dto.Telefone;
        profissional.RegistroProfissional = dto.RegistroProfissional;
        profissional.Especialidade = dto.Especialidade;
        profissional.UnidadeHospitalarId = dto.UnidadeHospitalarId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> InativarAsync(int id)
    {
        ProfissionalSaude? profissional = await _context.ProfissionaisSaude
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profissional == null)
        {
            return false;
        }

        profissional.Ativo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}