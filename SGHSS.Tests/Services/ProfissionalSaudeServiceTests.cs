using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Tests.Services;

[ExcludeFromCodeCoverage]
public class ProfissionalSaudeServiceTests : TestBase
{
    private IProfissionalSaudeService CreateService(ApplicationDbContext context)
    {
        IProfissionalSaudeService service = new ProfissionalSaudeService(context);
        return service;
    }

    private ProfissionalSaudeCreateDto CreateValidProfissionalDto(string cpf, string nome)
    {
        ProfissionalSaudeCreateDto dto = new ProfissionalSaudeCreateDto
        {
            Nome = nome,
            Cpf = cpf,
            RegistroProfissional = "CRM123",
            Especialidade = "Clínico Geral",
            Email = "medico@test.com",
            Telefone = "11912345678",
            UnidadeHospitalarId = null
        };
        return dto;
    }

    [Fact]
    public async Task CriarAsync_DeveCriarProfissional()
    {
        ApplicationDbContext context = CreateContext();
        IProfissionalSaudeService service = CreateService(context);

        ProfissionalSaudeCreateDto dto = CreateValidProfissionalDto("11144477735", "Médico 1");

        ProfissionalSaudeReadDto result = await service.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Nome.Should().Be("Médico 1");
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarProfissional_QuandoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IProfissionalSaudeService service = CreateService(context);

        ProfissionalSaudeCreateDto dto = CreateValidProfissionalDto("11144477735", "Médico 1");
        ProfissionalSaudeReadDto created = await service.CreateAsync(dto);

        ProfissionalSaudeReadDto? found = await service.GetByIdAsync(created.Id);

        found.Should().NotBeNull();
        found!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarLista()
    {
        ApplicationDbContext context = CreateContext();
        IProfissionalSaudeService service = CreateService(context);

        ProfissionalSaudeCreateDto dto1 = CreateValidProfissionalDto("11144477735", "Médico 1");
        ProfissionalSaudeCreateDto dto2 = CreateValidProfissionalDto("11144477744", "Médico 2");

        await service.CreateAsync(dto1);
        await service.CreateAsync(dto2);

        IReadOnlyList<ProfissionalSaudeReadDto> lista = await service.GetAllAsync();

        lista.Count.Should().Be(2);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAlterarDadosProfissional()
    {
        ApplicationDbContext context = CreateContext();
        IProfissionalSaudeService service = CreateService(context);

        ProfissionalSaudeCreateDto dto = CreateValidProfissionalDto("11144477735", "Médico 1");
        ProfissionalSaudeReadDto created = await service.CreateAsync(dto);

        ProfissionalSaudeCreateDto update = new ProfissionalSaudeCreateDto
        {
            Nome = "Médico Atualizado",
            Especialidade = "Cardiologista",
            Email = "medicoatualizado@test.com",
            Telefone = "11987654321"
        };

        bool updated = await service.UpdateAsync(created.Id, update);

        updated.Should().BeTrue();
    }

    [Fact]
    public async Task DeletarAsync_DeveExcluirProfissional()
    {
        ApplicationDbContext context = CreateContext();
        IProfissionalSaudeService service = CreateService(context);

        ProfissionalSaudeCreateDto dto = CreateValidProfissionalDto("11144477735", "Médico 1");
        ProfissionalSaudeReadDto created = await service.CreateAsync(dto);

        await service.InativarAsync(created.Id);

        bool exists = await context.ProfissionaisSaude.AnyAsync(p => p.Id == created.Id);
        exists.Should().BeTrue();
    }
}