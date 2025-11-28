using System;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Services;

public class ProntuarioService : IProntuarioService
{
    private readonly ApplicationDbContext _context;

    public ProntuarioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProntuarioReadDto?> GetByConsultaIdAsync(int consultaId)
    {
        Prontuario? prontuario = await _context.Prontuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ConsultaId == consultaId);

        if (prontuario == null)
        {
            return null;
        }

        ProntuarioReadDto dto = new ProntuarioReadDto
        {
            Id = prontuario.Id,
            ConsultaId = prontuario.ConsultaId,
            DataRegistro = prontuario.DataRegistro,
            Anotacoes = prontuario.Anotacoes,
            Diagnostico = prontuario.Diagnostico,
            PlanoTratamento = prontuario.PlanoTratamento
        };

        return dto;
    }

    public async Task<ProntuarioReadDto?> GetByIdAsync(int id)
    {
        Prontuario? prontuario = await _context.Prontuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prontuario == null)
        {
            return null;
        }

        ProntuarioReadDto dto = new ProntuarioReadDto
        {
            Id = prontuario.Id,
            ConsultaId = prontuario.ConsultaId,
            DataRegistro = prontuario.DataRegistro,
            Anotacoes = prontuario.Anotacoes,
            Diagnostico = prontuario.Diagnostico,
            PlanoTratamento = prontuario.PlanoTratamento
        };

        return dto;
    }

    public async Task<ProntuarioReadDto> CreateOrUpdateByConsultaAsync(ProntuarioCreateDto dto)
    {
        Consulta? consulta = await _context.Consultas
            .FirstOrDefaultAsync(c => c.Id == dto.ConsultaId);

        if (consulta == null)
        {
            throw new InvalidOperationException("Consulta não encontrada.");
        }

        Prontuario? prontuario = await _context.Prontuarios
            .FirstOrDefaultAsync(p => p.ConsultaId == dto.ConsultaId);

        if (prontuario == null)
        {
            prontuario = new Prontuario
            {
                ConsultaId = dto.ConsultaId,
                Anotacoes = dto.Anotacoes,
                Diagnostico = dto.Diagnostico,
                PlanoTratamento = dto.PlanoTratamento
            };

            _context.Prontuarios.Add(prontuario);
        }
        else
        {
            prontuario.Anotacoes = dto.Anotacoes;
            prontuario.Diagnostico = dto.Diagnostico;
            prontuario.PlanoTratamento = dto.PlanoTratamento;
        }

        await _context.SaveChangesAsync();

        ProntuarioReadDto result = new ProntuarioReadDto
        {
            Id = prontuario.Id,
            ConsultaId = prontuario.ConsultaId,
            DataRegistro = prontuario.DataRegistro,
            Anotacoes = prontuario.Anotacoes,
            Diagnostico = prontuario.Diagnostico,
            PlanoTratamento = prontuario.PlanoTratamento
        };

        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Prontuario? prontuario = await _context.Prontuarios
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prontuario == null)
        {
            return false;
        }

        _context.Prontuarios.Remove(prontuario);
        await _context.SaveChangesAsync();
        return true;
    }
}
