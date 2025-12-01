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
public class InternacaoServiceTests : TestBase
{
    private IInternacaoService CreateService(ApplicationDbContext context)
    {
        IInternacaoService service = new InternacaoService(context);
        return service;
    }

    private async Task<int> SeedPaciente(ApplicationDbContext context, bool ativo = true)
    {
        Paciente paciente = new Paciente
        {
            Nome = "Paciente Internacao 2",
            Cpf = "11144477735",
            DataNascimento = new DateTime(1990, 1, 1),
            Ativo = ativo,
            Email = "teste@gmail.com",
            Endereco = "Teste 123",
            Telefone = "88987657686"
        };
        context.Pacientes.Add(paciente);
        await context.SaveChangesAsync();
        return paciente.Id;
    }

    private async Task<int> SeedLeito(ApplicationDbContext context, StatusLeito status = StatusLeito.Livre)
    {
        Leito leito = new Leito
        {
            Codigo = "L999",
            Tipo = "UTI",
            Status = status
        };
        context.Leitos.Add(leito);
        await context.SaveChangesAsync();
        return leito.Id;
    }

    private async Task<int> SeedInternacao(ApplicationDbContext context, int pacienteId, int leitoId)
    {
        Internacao internacao = new Internacao
        {
            PacienteId = pacienteId,
            LeitoId = leitoId,
            Motivo = "Teste",
            Status = StatusInternacao.Ativa,
            DataEntrada = DateTime.UtcNow
        };
        context.Internacoes.Add(internacao);

        // ocupar leito
        Leito leito = await context.Leitos.FirstAsync(l => l.Id == leitoId);
        leito.Status = StatusLeito.Ocupado;

        await context.SaveChangesAsync();
        return internacao.Id;
    }

    [Fact]
    public async Task InternarAsync_DeveLancar_QuandoPacienteInexistenteOuInativo()
    {
        ApplicationDbContext context = CreateContext();
        IInternacaoService service = CreateService(context);

        int leitoId = await SeedLeito(context);

        InternacaoCreateDto dto = new InternacaoCreateDto
        {
            PacienteId = 9999, // nao existe
            LeitoId = leitoId,
            Motivo = "Motivo"
        };

        Func<Task> act1 = async () => await service.InternarAsync(dto);
        await act1.Should().ThrowAsync<InvalidOperationException>().WithMessage("Paciente não encontrado ou inativo.");

        // paciente inativo
        int pacienteId = await SeedPaciente(context, ativo: false);

        InternacaoCreateDto dto2 = new InternacaoCreateDto
        {
            PacienteId = pacienteId,
            LeitoId = leitoId,
            Motivo = "Motivo"
        };

        Func<Task> act2 = async () => await service.InternarAsync(dto2);
        await act2.Should().ThrowAsync<InvalidOperationException>().WithMessage("Paciente não encontrado ou inativo.");
    }

    [Fact]
    public async Task InternarAsync_DeveLancar_QuandoLeitoNaoExisteOuIndisponivel()
    {
        ApplicationDbContext context = CreateContext();
        IInternacaoService service = CreateService(context);

        int pacienteId = await SeedPaciente(context);
        // leito inexistente
        InternacaoCreateDto dto = new InternacaoCreateDto
        {
            PacienteId = pacienteId,
            LeitoId = 9999,
            Motivo = "Motivo"
        };

        Func<Task> act1 = async () => await service.InternarAsync(dto);
        await act1.Should().ThrowAsync<InvalidOperationException>().WithMessage("Leito não encontrado.");

        // leito ocupado
        int leitoId = await SeedLeito(context, StatusLeito.Ocupado);

        InternacaoCreateDto dto2 = new InternacaoCreateDto
        {
            PacienteId = pacienteId,
            LeitoId = leitoId,
            Motivo = "Motivo"
        };

        Func<Task> act2 = async () => await service.InternarAsync(dto2);
        await act2.Should().ThrowAsync<InvalidOperationException>().WithMessage("Leito não está disponível.");
    }

    [Fact]
    public async Task TransferirAsync_DeveRetornarFalse_QuandoInternacaoNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IInternacaoService service = CreateService(context);

        bool result = await service.TransferirAsync(9999, 1);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task TransferirAsync_DeveLancar_QuandoNovoLeitoInvalidoOuIndisponivel()
    {
        ApplicationDbContext context = CreateContext();
        IInternacaoService service = CreateService(context);

        int pacienteId = await SeedPaciente(context);
        int leitoAId = await SeedLeito(context);
        int leitoBId = await SeedLeito(context, StatusLeito.Ocupado);

        int internacaoId = await SeedInternacao(context, pacienteId, leitoAId);

        // novo leito inexistente
        Func<Task> act1 = async () => await service.TransferirAsync(internacaoId, 9999);
        await act1.Should().ThrowAsync<InvalidOperationException>().WithMessage("Novo leito inválido ou indisponível.");

        // novo leito ocupado
        Func<Task> act2 = async () => await service.TransferirAsync(internacaoId, leitoBId);
        await act2.Should().ThrowAsync<InvalidOperationException>().WithMessage("Novo leito inválido ou indisponível.");
    }

    [Fact]
    public async Task TransferirAsync_DeveTransferirComSucesso()
    {
        ApplicationDbContext context = CreateContext();
        IInternacaoService service = CreateService(context);

        int pacienteId = await SeedPaciente(context);
        int leitoAId = await SeedLeito(context);
        int leitoBId = await SeedLeito(context);

        int internacaoId = await SeedInternacao(context, pacienteId, leitoAId);

        bool ok = await service.TransferirAsync(internacaoId, leitoBId);
        ok.Should().BeTrue();

        Leito leitoA = await context.Leitos.FirstAsync(l => l.Id == leitoAId);
        Leito leitoB = await context.Leitos.FirstAsync(l => l.Id == leitoBId);

        leitoA.Status.Should().Be(StatusLeito.Livre);
        leitoB.Status.Should().Be(StatusLeito.Ocupado);

        Internacao? internacao = await context.Internacoes.Include(i => i.Leito).FirstOrDefaultAsync(i => i.Id == internacaoId);
        internacao!.LeitoId.Should().Be(leitoBId);
    }

    [Fact]
    public async Task DarAltaAsync_DeveRetornarFalse_QuandoNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IInternacaoService service = CreateService(context);

        bool result = await service.DarAltaAsync(9999);
        result.Should().BeFalse();
    }
}