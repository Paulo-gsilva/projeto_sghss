using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Tests.Services;

[ExcludeFromCodeCoverage]
public class ReceitaDigitalServiceTests : TestBase
{
    private IReceitaService CreateService(ApplicationDbContext context)
    {
        IReceitaService service = new ReceitaService(context);
        return service;
    }

    private async Task<int> SeedConsultaFinalizada(ApplicationDbContext context)
    {
        Paciente paciente = new Paciente
        {
            Nome = "Paciente Receita",
            Cpf = "11144477735",
            DataNascimento = new DateTime(1990, 1, 1),
            Ativo = true,
            Email = "teste@gmail.com",
            Endereco = "Teste 123",
            Telefone = "88987657686"
        };
        context.Pacientes.Add(paciente);

        ProfissionalSaude profissional = new ProfissionalSaude
        {
            Nome = "Medico Receita",
            Cpf = "11144477744",
            RegistroProfissional = "CRM-123",
            Especialidade = "Clínico",
            Email = "teste@gmail.com",
            Telefone = "88987657686"
        };
        context.ProfissionaisSaude.Add(profissional);

        await context.SaveChangesAsync();

        Consulta consulta = new Consulta
        {
            PacienteId = paciente.Id,
            ProfissionalSaudeId = profissional.Id,
            DataHora = DateTime.UtcNow.AddDays(-1),
            Tipo = TipoConsulta.Presencial,
            Motivo = "Checagem",
            Status = StatusConsulta.Concluida
        };
        context.Consultas.Add(consulta);
        await context.SaveChangesAsync();

        return consulta.Id;
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IReceitaService service = CreateService(context);

        ReceitaReadDto? found = await service.GetByIdAsync(9999);
        found.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_DeveLancar_QuandoConsultaNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IReceitaService service = CreateService(context);

        ReceitaCreateDto dto = new ReceitaCreateDto
        {
            ConsultaId = 9999,
            Observacoes = "Obs",
            ValidaAte = null,
            Medicamentos = new List<MedicamentoCreateDto>()
        };

        Func<Task> act = async () => await service.CreateAsync(dto);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Consulta não encontrada.");
    }

    [Fact]
    public async Task CreateAsync_DeveCriarReceitaEAssociarConsulta()
    {
        ApplicationDbContext context = CreateContext();
        IReceitaService service = CreateService(context);

        int consultaId = await SeedConsultaFinalizada(context);

        ReceitaCreateDto dto = new ReceitaCreateDto
        {
            ConsultaId = consultaId,
            Observacoes = "Usar conforme prescrição",
            ValidaAte = DateTime.UtcNow.AddDays(7),
            Medicamentos = new List<MedicamentoCreateDto>
            {
                new MedicamentoCreateDto
                {
                    NomeMedicamento = "Remedio A",
                    Dosagem = "500mg",
                    Frequencia = "3x ao dia",
                    Duracao = "7 dias"
                }
            }
        };

        ReceitaReadDto created = await service.CreateAsync(dto);

        created.Id.Should().BeGreaterThan(0);
        created.Medicamentos.Should().HaveCount(1);
        created.CodigoValidacao.Should().NotBeNullOrEmpty();

        Consulta? consulta = await context.Consultas.FirstOrDefaultAsync(c => c.Id == consultaId);
        consulta!.ReceitaDigitalId.Should().Be(created.Id);
    }

    [Fact]
    public async Task AddMedicamentoAsync_DeveRetornarFalse_QuandoReceitaNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IReceitaService service = CreateService(context);

        MedicamentoCreateDto medDto = new MedicamentoCreateDto
        {
            NomeMedicamento = "X",
            Dosagem = "1",
            Frequencia = "1x",
            Duracao = "1 dia"
        };

        bool result = await service.AddMedicamentoAsync(9999, medDto);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddMedicamentoAsync_DeveAdicionarMedicamento()
    {
        ApplicationDbContext context = CreateContext();
        IReceitaService service = CreateService(context);

        int consultaId = await SeedConsultaFinalizada(context);

        // criar receita primeiro
        ReceitaCreateDto dto = new ReceitaCreateDto
        {
            ConsultaId = consultaId,
            Medicamentos = new List<MedicamentoCreateDto>()
        };

        ReceitaReadDto created = await service.CreateAsync(dto);

        MedicamentoCreateDto medDto = new MedicamentoCreateDto
        {
            NomeMedicamento = "NovoMed",
            Dosagem = "10mg",
            Frequencia = "2x",
            Duracao = "5 dias"
        };

        bool added = await service.AddMedicamentoAsync(created.Id, medDto);
        added.Should().BeTrue();

        ReceitaDigital? r = await context.ReceitasDigitais.Include(x => x.Medicamentos).FirstOrDefaultAsync(x => x.Id == created.Id);
        r!.Medicamentos.Should().HaveCount(1);
        r.Medicamentos.First().NomeMedicamento.Should().Be("NovoMed");
    }

    [Fact]
    public async Task RemoveMedicamentoAsync_DeveRetornarFalse_QuandoNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IReceitaService service = CreateService(context);

        bool removed = await service.RemoveMedicamentoAsync(9999, 1);
        removed.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveMedicamentoAsync_DeveRemoverMedicamento()
    {
        ApplicationDbContext context = CreateContext();
        IReceitaService service = CreateService(context);

        int consultaId = await SeedConsultaFinalizada(context);

        ReceitaCreateDto dto = new ReceitaCreateDto
        {
            ConsultaId = consultaId,
            Medicamentos = new List<MedicamentoCreateDto>
            {
                new MedicamentoCreateDto
                {
                    NomeMedicamento = "M1",
                    Dosagem = "1",
                    Frequencia = "1x",
                    Duracao = "2 dias"
                }
            }
        };

        ReceitaReadDto created = await service.CreateAsync(dto);

        ReceitaDigital? rBefore = await context.ReceitasDigitais.Include(r => r.Medicamentos).FirstOrDefaultAsync(r => r.Id == created.Id);
        int medicamentoId = rBefore!.Medicamentos.First().Id;

        bool removed = await service.RemoveMedicamentoAsync(created.Id, medicamentoId);
        removed.Should().BeTrue();

        ReceitaDigital? rAfter = await context.ReceitasDigitais.Include(r => r.Medicamentos).FirstOrDefaultAsync(r => r.Id == created.Id);
        rAfter!.Medicamentos.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidarCodigoAsync_DeveRetornarFalse_QuandoNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IReceitaService service = CreateService(context);

        bool ok = await service.ValidarCodigoAsync("NAOEXISTE");
        ok.Should().BeFalse();
    }

    [Fact]
    public async Task ValidarCodigoAsync_DeveRetornarFalse_QuandoExpirado()
    {
        ApplicationDbContext context = CreateContext();
        IReceitaService service = CreateService(context);

        ReceitaDigital receita = new ReceitaDigital
        {
            DataEmissao = DateTime.UtcNow.AddDays(-10),
            Observacoes = "teste",
            CodigoValidacao = "CODEEXPIRED",
            ValidaAte = DateTime.UtcNow.AddDays(-1),
            Medicamentos = new List<MedicamentoPrescrito>()
        };
        context.ReceitasDigitais.Add(receita);
        await context.SaveChangesAsync();

        bool ok = await service.ValidarCodigoAsync("CODEEXPIRED");
        ok.Should().BeFalse();
    }

    [Fact]
    public async Task ValidarCodigoAsync_DeveRetornarTrue_QuandoValido()
    {
        ApplicationDbContext context = CreateContext();
        IReceitaService service = CreateService(context);

        ReceitaDigital receita = new ReceitaDigital
        {
            DataEmissao = DateTime.UtcNow,
            Observacoes = "teste",
            CodigoValidacao = "CODEOK",
            ValidaAte = DateTime.UtcNow.AddDays(10),
            Medicamentos = new List<MedicamentoPrescrito>()
        };
        context.ReceitasDigitais.Add(receita);
        await context.SaveChangesAsync();

        bool ok = await service.ValidarCodigoAsync("CODEOK");
        ok.Should().BeTrue();
    }
}