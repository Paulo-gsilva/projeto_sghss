using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Tests.Services;

[ExcludeFromCodeCoverage]
public class ProntuarioServiceTests : TestBase
{
    private IProntuarioService CreateService(ApplicationDbContext context)
    {
        IProntuarioService service = new ProntuarioService(context);
        return service;
    }

    private async Task<int> SeedConsulta(ApplicationDbContext context, bool finalize = false)
    {
        Paciente paciente = new Paciente
        {
            Nome = "Paciente Prontuario",
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
            Nome = "Profissional",
            Cpf = "11144477744",
            RegistroProfissional = "CRM-999",
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
            DataHora = DateTime.UtcNow,
            Tipo = TipoConsulta.Presencial,
            Motivo = "Checagem",
            Status = finalize ? StatusConsulta.Concluida : StatusConsulta.Agendada
        };
        context.Consultas.Add(consulta);
        await context.SaveChangesAsync();

        return consulta.Id;
    }

    [Fact]
    public async Task GetByConsultaIdAsync_DeveRetornarNull_QuandoNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IProntuarioService service = CreateService(context);

        ProntuarioReadDto? result = await service.GetByConsultaIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateOrUpdateByConsultaAsync_DeveLancar_QuandoConsultaNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IProntuarioService service = CreateService(context);

        ProntuarioCreateDto dto = new ProntuarioCreateDto
        {
            ConsultaId = 9999,
            Anotacoes = "Texto",
            Diagnostico = "Dx",
            PlanoTratamento = "Plano"
        };

        Func<Task> act = async () => await service.CreateOrUpdateByConsultaAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Consulta não encontrada.");
    }

    [Fact]
    public async Task CreateOrUpdateByConsultaAsync_DeveCriarProntuario()
    {
        ApplicationDbContext context = CreateContext();
        IProntuarioService service = CreateService(context);

        int consultaId = await SeedConsulta(context, finalize: true);

        ProntuarioCreateDto dto = new ProntuarioCreateDto
        {
            ConsultaId = consultaId,
            Anotacoes = "Paciente está bem",
            Diagnostico = "Sem alterações",
            PlanoTratamento = "Repouso"
        };

        ProntuarioReadDto created = await service.CreateOrUpdateByConsultaAsync(dto);

        created.Id.Should().BeGreaterThan(0);
        created.ConsultaId.Should().Be(consultaId);

        ProntuarioReadDto? fetched = await service.GetByConsultaIdAsync(consultaId);
        fetched.Should().NotBeNull();
        fetched!.Diagnostico.Should().Be("Sem alterações");
    }

    [Fact]
    public async Task CreateOrUpdateByConsultaAsync_DeveAtualizarProntuarioExistente()
    {
        ApplicationDbContext context = CreateContext();
        IProntuarioService service = CreateService(context);

        int consultaId = await SeedConsulta(context, finalize: true);

        ProntuarioCreateDto dto1 = new ProntuarioCreateDto
        {
            ConsultaId = consultaId,
            Anotacoes = "A",
            Diagnostico = "Dx A",
            PlanoTratamento = "P1"
        };

        ProntuarioReadDto created = await service.CreateOrUpdateByConsultaAsync(dto1);

        ProntuarioCreateDto dto2 = new ProntuarioCreateDto
        {
            ConsultaId = consultaId,
            Anotacoes = "B - atualizado",
            Diagnostico = "Dx B",
            PlanoTratamento = "P2"
        };

        ProntuarioReadDto updated = await service.CreateOrUpdateByConsultaAsync(dto2);

        updated.Id.Should().Be(created.Id);
        updated.Anotacoes.Should().Be("B - atualizado");
        updated.Diagnostico.Should().Be("Dx B");
    }

    [Fact]
    public async Task DeleteAsync_DeveRetornarFalse_QuandoNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IProntuarioService service = CreateService(context);

        bool removed = await service.DeleteAsync(9999);
        removed.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_DeveRemoverProntuarioExistente()
    {
        ApplicationDbContext context = CreateContext();
        IProntuarioService service = CreateService(context);

        int consultaId = await SeedConsulta(context, finalize: true);

        ProntuarioCreateDto dto = new ProntuarioCreateDto
        {
            ConsultaId = consultaId,
            Anotacoes = "X",
            Diagnostico = "DxX",
            PlanoTratamento = "PX"
        };

        ProntuarioReadDto created = await service.CreateOrUpdateByConsultaAsync(dto);

        bool removed = await service.DeleteAsync(created.Id);
        removed.Should().BeTrue();

        ProntuarioReadDto? fetched = await service.GetByIdAsync(created.Id);
        fetched.Should().BeNull();
    }
}