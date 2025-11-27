using System;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Services;

public class PacienteService : IPacienteService
{
    private readonly ApplicationDbContext _context;

    public PacienteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PacienteReadDto>> GetAllAsync()
    {
        List<Paciente> pacientes = await _context.Pacientes
            .AsNoTracking()
            .ToListAsync();

        List<PacienteReadDto> result = pacientes
            .Select(p => new PacienteReadDto(p))
            .ToList();

        return result;
    }

    public async Task<PacienteReadDto?> GetByIdAsync(int id)
    {
        Paciente? paciente = await _context.Pacientes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (paciente == null)
        {
            return null;
        }

        PacienteReadDto dto = new PacienteReadDto(paciente);
        return dto;
    }

    public async Task<PacienteReadDto> CreateAsync(PacienteCreateDto dto)
    {
        bool cpfExists = await _context.Pacientes
            .AnyAsync(x => x.Cpf == dto.Cpf);

        if (cpfExists)
        {
            throw new System.InvalidOperationException("CPF já cadastrado.");
        }

        Paciente paciente = new Paciente
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            DataNascimento = dto.DataNascimento,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Endereco = dto.Endereco,
            Ativo = true
        };

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        PacienteReadDto result = new PacienteReadDto(paciente);
        return result;
    }

    public async Task<bool> UpdateAsync(int id, PacienteCreateDto dto)
    {
        Paciente? paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id == id);

        if (paciente == null)
        {
            return false;
        }

        paciente.Nome = dto.Nome;
        paciente.Email = dto.Email;
        paciente.Telefone = dto.Telefone;
        paciente.Endereco = dto.Endereco;
        paciente.DataNascimento = dto.DataNascimento;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> InativarAsync(int id)
    {
        Paciente? paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id == id);

        if (paciente == null)
        {
            return false;
        }

        paciente.Ativo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}