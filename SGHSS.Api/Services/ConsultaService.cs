using System;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Services;

public class ConsultaService : IConsultaService
{
    private readonly ApplicationDbContext _context;

    public ConsultaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ConsultaReadDto>> GetAllAsync()
    {
        List<Consulta> consultas = await _context.Consultas
            .Include(c => c.Paciente)
            .Include(c => c.ProfissionalSaude)
            .AsNoTracking()
            .ToListAsync();

        List<ConsultaReadDto> result = consultas
            .Select(c => new ConsultaReadDto(c))
            .ToList();

        return result;
    }

    public async Task<ConsultaReadDto?> GetByIdAsync(int id)
    {
        Consulta? consulta = await _context.Consultas
            .Include(c => c.Paciente)
            .Include(c => c.ProfissionalSaude)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (consulta == null)
        {
            return null;
        }

        ConsultaReadDto dto = new ConsultaReadDto(consulta);
        return dto;
    }

    public async Task<ConsultaReadDto> AgendarAsync(ConsultaCreateDto dto)
    {
        bool pacienteExiste = await _context.Pacientes.AnyAsync(p => p.Id == dto.PacienteId && p.Ativo);
        if (!pacienteExiste)
        {
            throw new System.InvalidOperationException("Paciente não encontrado ou inativo.");
        }

        bool medicoExiste = await _context.ProfissionaisSaude.AnyAsync(p => p.Id == dto.ProfissionalSaudeId && p.Ativo);
        if (!medicoExiste)
        {
            throw new System.InvalidOperationException("Profissional de saúde não encontrado ou inativo.");
        }

        Consulta consulta = new Consulta
        {
            PacienteId = dto.PacienteId,
            ProfissionalSaudeId = dto.ProfissionalSaudeId,
            DataHora = dto.DataHora,
            Tipo = dto.Tipo,
            Status = StatusConsulta.Agendada,
            Motivo = dto.Motivo
        };

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        Consulta consultaCarregada = await _context.Consultas
            .Include(c => c.Paciente)
            .Include(c => c.ProfissionalSaude)
            .FirstAsync(c => c.Id == consulta.Id);

        ConsultaReadDto result = new ConsultaReadDto(consultaCarregada);
        return result;
    }

    public async Task<bool> CancelarAsync(int id)
    {
        Consulta? consulta = await _context.Consultas
            .FirstOrDefaultAsync(c => c.Id == id);

        if (consulta == null)
        {
            return false;
        }

        consulta.Status = StatusConsulta.Cancelada;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ConcluirAsync(int id)
    {
        Consulta? consulta = await _context.Consultas
            .FirstOrDefaultAsync(c => c.Id == id);

        if (consulta == null)
        {
            return false;
        }

        consulta.Status = StatusConsulta.Concluida;
        await _context.SaveChangesAsync();
        return true;
    }
}