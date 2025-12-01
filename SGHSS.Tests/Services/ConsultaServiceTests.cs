using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Moq;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Tests.Services;

[ExcludeFromCodeCoverage]
public class ConsultaServiceTests : TestBase
{
    private IConsultaService CreateService(ApplicationDbContext context)
    {
        var prontuarioServiceMock = new Mock<IProntuarioService>();

        IConsultaService service = new ConsultaService(
            context,
            prontuarioServiceMock.Object
        );

        return service;
    }

    private async Task<int> SeedPaciente(ApplicationDbContext context)
    {
        Paciente paciente = new Paciente
        {
            Nome = "Paciente",
            Cpf = "11144477735",
            DataNascimento = new DateTime(1990, 1, 1),
            Email = "teste@gmail.com",
            Endereco = "Teste 123",
            Telefone = "88987657686"
        };
        context.Pacientes.Add(paciente);
        await context.SaveChangesAsync();
        return paciente.Id;
    }

    private async Task<int> SeedProfissional(ApplicationDbContext context)
    {
        ProfissionalSaude profissional = new ProfissionalSaude
        {
            Nome = "Médico",
            Cpf = "11144477744",
            RegistroProfissional = "CRM123",
            Especialidade = "Clínico Geral",
            Email = "teste@gmail.com",
            Telefone = "88987657686"
        };
        context.ProfissionaisSaude.Add(profissional);
        await context.SaveChangesAsync();
        return profissional.Id;
    }

    [Fact]
    public async Task AgendarAsync_DeveCriarConsultaAgendada()
    {
        ApplicationDbContext context = CreateContext();
        IConsultaService service = CreateService(context);

        int pacienteId = await SeedPaciente(context);
        int profissionalId = await SeedProfissional(context);

        ConsultaCreateDto dto = new ConsultaCreateDto
        {
            PacienteId = pacienteId,
            ProfissionalSaudeId = profissionalId,
            DataHora = DateTime.UtcNow.AddDays(1),
            Tipo = TipoConsulta.Presencial,
            Motivo = "Rotina"
        };

        ConsultaReadDto result = await service.AgendarAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Status.Should().Be(StatusConsulta.Agendada);
    }

    [Fact]
    public async Task CancelarAsync_DeveAlterarStatusParaCancelada()
    {
        ApplicationDbContext context = CreateContext();
        IConsultaService service = CreateService(context);

        int pacienteId = await SeedPaciente(context);
        int profissionalId = await SeedProfissional(context);

        ConsultaCreateDto dto = new ConsultaCreateDto
        {
            PacienteId = pacienteId,
            ProfissionalSaudeId = profissionalId,
            DataHora = DateTime.UtcNow.AddDays(1),
            Tipo = TipoConsulta.Presencial,
            Motivo = "Rotina"
        };

        ConsultaReadDto created = await service.AgendarAsync(dto);

        await service.CancelarAsync(created.Id);

        ConsultaReadDto? updated = await service.GetByIdAsync(created.Id);
        updated!.Status.Should().Be(StatusConsulta.Cancelada);
    }

    [Fact]
    public async Task FinalizarAsync_DeveAlterarStatusParaFinalizada()
    {
        ApplicationDbContext context = CreateContext();
        IConsultaService service = CreateService(context);

        int pacienteId = await SeedPaciente(context);
        int profissionalId = await SeedProfissional(context);

        ConsultaCreateDto dto = new ConsultaCreateDto
        {
            PacienteId = pacienteId,
            ProfissionalSaudeId = profissionalId,
            DataHora = DateTime.UtcNow.AddDays(-1),
            Tipo = TipoConsulta.Presencial,
            Motivo = "Rotina"
        };

        ConsultaReadDto created = await service.AgendarAsync(dto);

        await service.ConcluirAsync(created.Id);

        ConsultaReadDto? updated = await service.GetByIdAsync(created.Id);
        updated!.Status.Should().Be(StatusConsulta.Concluida);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarListaConsultas()
    {
        ApplicationDbContext context = CreateContext();
        IConsultaService service = CreateService(context);

        int pacienteId = await SeedPaciente(context);
        int profissionalId = await SeedProfissional(context);

        ConsultaCreateDto dto1 = new ConsultaCreateDto
        {
            PacienteId = pacienteId,
            ProfissionalSaudeId = profissionalId,
            DataHora = DateTime.UtcNow.AddDays(1),
            Tipo = TipoConsulta.Presencial,
            Motivo = "Consulta 1"
        };

        ConsultaCreateDto dto2 = new ConsultaCreateDto
        {
            PacienteId = pacienteId,
            ProfissionalSaudeId = profissionalId,
            DataHora = DateTime.UtcNow.AddDays(2),
            Tipo = TipoConsulta.Teleconsulta,
            Motivo = "Consulta 2"
        };

        await service.AgendarAsync(dto1);
        await service.AgendarAsync(dto2);

        IReadOnlyList<ConsultaReadDto> list = await service.GetAllAsync();

        list.Count.Should().Be(2);
    }
}
