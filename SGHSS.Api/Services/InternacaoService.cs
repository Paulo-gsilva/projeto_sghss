using System;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Services;

public class InternacaoService : IInternacaoService
{
    private readonly ApplicationDbContext _context;

    public InternacaoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<InternacaoReadDto>> GetAllAsync()
    {
        List<Internacao> internacoes = await _context.Internacoes
            .Include(i => i.Paciente)
            .Include(i => i.Leito)
            .AsNoTracking()
            .ToListAsync();

        List<InternacaoReadDto> result = internacoes
            .Select(i => new InternacaoReadDto(i))
            .ToList();

        return result;
    }

    public async Task<InternacaoReadDto?> GetByIdAsync(int id)
    {
        Internacao? internacao = await _context.Internacoes
            .Include(i => i.Paciente)
            .Include(i => i.Leito)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (internacao == null)
        {
            return null;
        }

        InternacaoReadDto dto = new InternacaoReadDto(internacao);
        return dto;
    }

    public async Task<InternacaoReadDto> InternarAsync(InternacaoCreateDto dto)
    {
        bool pacienteExiste = await _context.Pacientes.AnyAsync(p => p.Id == dto.PacienteId && p.Ativo);
        if (!pacienteExiste)
        {
            throw new System.InvalidOperationException("Paciente não encontrado ou inativo.");
        }

        Leito? leito = await _context.Leitos.FirstOrDefaultAsync(l => l.Id == dto.LeitoId);
        if (leito == null)
        {
            throw new System.InvalidOperationException("Leito não encontrado.");
        }

        if (leito.Status != StatusLeito.Livre)
        {
            throw new System.InvalidOperationException("Leito não está disponível.");
        }

        Internacao internacao = new Internacao
        {
            PacienteId = dto.PacienteId,
            LeitoId = dto.LeitoId,
            Motivo = dto.Motivo,
            Status = StatusInternacao.Ativa
        };

        leito.Status = StatusLeito.Ocupado;

        _context.Internacoes.Add(internacao);
        await _context.SaveChangesAsync();

        Internacao internacaoCarregada = await _context.Internacoes
            .Include(i => i.Paciente)
            .Include(i => i.Leito)
            .FirstAsync(i => i.Id == internacao.Id);

        InternacaoReadDto result = new InternacaoReadDto(internacaoCarregada);
        return result;
    }

    public async Task<bool> TransferirAsync(int internacaoId, int novoLeitoId)
    {
        Internacao? internacao = await _context.Internacoes
            .Include(i => i.Leito)
            .FirstOrDefaultAsync(i => i.Id == internacaoId);

        if (internacao == null)
        {
            return false;
        }

        Leito? novoLeito = await _context.Leitos.FirstOrDefaultAsync(l => l.Id == novoLeitoId);
        if (novoLeito == null || novoLeito.Status != StatusLeito.Livre)
        {
            throw new System.InvalidOperationException("Novo leito inválido ou indisponível.");
        }

        Leito leitoAtual = internacao.Leito;
        leitoAtual.Status = StatusLeito.Livre;

        internacao.LeitoId = novoLeitoId;
        novoLeito.Status = StatusLeito.Ocupado;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DarAltaAsync(int internacaoId)
    {
        Internacao? internacao = await _context.Internacoes
            .Include(i => i.Leito)
            .FirstOrDefaultAsync(i => i.Id == internacaoId);

        if (internacao == null)
        {
            return false;
        }

        internacao.Status = StatusInternacao.Alta;
        internacao.DataSaida = System.DateTime.UtcNow;

        Leito leito = internacao.Leito;
        leito.Status = StatusLeito.Livre;

        await _context.SaveChangesAsync();
        return true;
    }
}