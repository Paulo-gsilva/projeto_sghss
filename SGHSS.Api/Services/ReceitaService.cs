using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Services;

public class ReceitaService : IReceitaService
{
    private readonly ApplicationDbContext _context;

    public ReceitaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReceitaReadDto?> GetByIdAsync(int id)
    {
        ReceitaDigital? receita = await _context.ReceitasDigitais
            .Include(r => r.Medicamentos)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receita == null)
        {
            return null;
        }

        ReceitaReadDto dto = MapToReadDto(receita);
        return dto;
    }

    public async Task<IReadOnlyList<ReceitaReadDto>> GetAllByConsultaAsync(int consultaId)
    {
        List<ReceitaDigital> list = await _context.ReceitasDigitais
            .Include(r => r.Medicamentos)
            .Where(r => r.Id == r.Id && r.Id != 0) 
            .ToListAsync();

        List<ReceitaReadDto> result = list.Select(r => MapToReadDto(r)).ToList();
        return result;
    }

    public async Task<ReceitaReadDto> CreateAsync(ReceitaCreateDto dto)
    {
        Consulta? consulta = await _context.Consultas
            .FirstOrDefaultAsync(c => c.Id == dto.ConsultaId);

        if (consulta == null)
        {
            throw new InvalidOperationException("Consulta não encontrada.");
        }

        DateTime dataValidade = dto.ValidaAte ?? DateTime.UtcNow.AddDays(30);

        ReceitaDigital receita = new ReceitaDigital
        {
            DataEmissao = DateTime.UtcNow,
            Observacoes = dto.Observacoes,
            CodigoValidacao = GenerateCodigoValidacao(),
            ValidaAte = dataValidade,
            Medicamentos = new List<MedicamentoPrescrito>()
        };

        foreach (MedicamentoCreateDto medDto in dto.Medicamentos)
        {
            MedicamentoPrescrito medicamento = new MedicamentoPrescrito
            {
                NomeMedicamento = medDto.NomeMedicamento,
                Dosagem = medDto.Dosagem,
                Frequencia = medDto.Frequencia,
                Duracao = medDto.Duracao
            };

            receita.Medicamentos.Add(medicamento);
        }

        _context.ReceitasDigitais.Add(receita);
        await _context.SaveChangesAsync();

        consulta.ReceitaDigitalId = receita.Id;
        await _context.SaveChangesAsync();

        ReceitaReadDto result = MapToReadDto(receita);
        return result;
    }

    public async Task<bool> AddMedicamentoAsync(int receitaId, MedicamentoCreateDto medicamentoDto)
    {
        ReceitaDigital? receita = await _context.ReceitasDigitais
            .Include(r => r.Medicamentos)
            .FirstOrDefaultAsync(r => r.Id == receitaId);

        if (receita == null)
        {
            return false;
        }

        MedicamentoPrescrito medicamento = new MedicamentoPrescrito
        {
            NomeMedicamento = medicamentoDto.NomeMedicamento,
            Dosagem = medicamentoDto.Dosagem,
            Frequencia = medicamentoDto.Frequencia,
            Duracao = medicamentoDto.Duracao,
            ReceitaDigital = receita
        };

        _context.MedicamentosPrescritos.Add(medicamento);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveMedicamentoAsync(int receitaId, int medicamentoId)
    {
        MedicamentoPrescrito? medicamento = await _context.MedicamentosPrescritos
            .FirstOrDefaultAsync(m => m.Id == medicamentoId && m.ReceitaDigitalId == receitaId);

        if (medicamento == null)
        {
            return false;
        }

        _context.MedicamentosPrescritos.Remove(medicamento);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidarCodigoAsync(string codigo)
    {
        ReceitaDigital? receita = await _context.ReceitasDigitais
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.CodigoValidacao == codigo);

        if (receita == null)
        {
            return false;
        }

        if (receita.ValidaAte < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    private static string GenerateCodigoValidacao()
    {
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[8];
            rng.GetBytes(bytes);
            string code = BitConverter.ToString(bytes).Replace("-", string.Empty).ToUpperInvariant();
            return code;
        }
    }

    private static ReceitaReadDto MapToReadDto(ReceitaDigital receita)
    {
        ReceitaReadDto dto = new ReceitaReadDto
        {
            Id = receita.Id,
            ConsultaId = 0,
            DataEmissao = receita.DataEmissao,
            Observacoes = receita.Observacoes,
            CodigoValidacao = receita.CodigoValidacao,
            ValidaAte = receita.ValidaAte,
            Medicamentos = receita.Medicamentos != null
                ? receita.Medicamentos.Select(m => new MedicamentoReadDto
                {
                    Id = m.Id,
                    NomeMedicamento = m.NomeMedicamento,
                    Dosagem = m.Dosagem,
                    Frequencia = m.Frequencia,
                    Duracao = m.Duracao
                }).ToList()
                : new List<MedicamentoReadDto>()
        };

        return dto;
    }
}