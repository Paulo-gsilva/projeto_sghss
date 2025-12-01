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
public class PacienteServiceTests : TestBase
{
    private IPacienteService CreateService(ApplicationDbContext context)
    {
        IPacienteService service = new PacienteService(context);
        return service;
    }

    private PacienteCreateDto CreateValidPacienteDto(string cpf, string nome)
    {
        PacienteCreateDto dto = new PacienteCreateDto
        {
            Nome = nome,
            Cpf = cpf,
            DataNascimento = new DateTime(1990, 1, 1),
            Email = "paciente@test.com",
            Telefone = "11912345678",
            Endereco = "Rua Teste, 123"
        };
        return dto;
    }

    [Fact]
    public async Task CriarAsync_DeveCriarPaciente()
    {
        ApplicationDbContext context = CreateContext();
        IPacienteService service = CreateService(context);

        PacienteCreateDto dto = CreateValidPacienteDto("11144477735", "Paciente 1");

        PacienteReadDto result = await service.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Nome.Should().Be("Paciente 1");
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarPaciente_QuandoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IPacienteService service = CreateService(context);

        PacienteCreateDto dto = CreateValidPacienteDto("11144477735", "Paciente 1");
        PacienteReadDto created = await service.CreateAsync(dto);

        PacienteReadDto? found = await service.GetByIdAsync(created.Id);

        found.Should().NotBeNull();
        found!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IPacienteService service = CreateService(context);

        PacienteReadDto? found = await service.GetByIdAsync(999);

        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarLista()
    {
        ApplicationDbContext context = CreateContext();
        IPacienteService service = CreateService(context);

        PacienteCreateDto dto1 = CreateValidPacienteDto("11144477735", "Paciente 1");
        PacienteCreateDto dto2 = CreateValidPacienteDto("11144477744", "Paciente 2");

        await service.CreateAsync(dto1);
        await service.CreateAsync(dto2);

        IReadOnlyList<PacienteReadDto> lista = await service.GetAllAsync();

        lista.Count.Should().Be(2);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAlterarDadosPaciente()
    {
        ApplicationDbContext context = CreateContext();
        IPacienteService service = CreateService(context);

        PacienteCreateDto dto = CreateValidPacienteDto("11144477735", "Paciente 1");
        PacienteReadDto created = await service.CreateAsync(dto);

        PacienteCreateDto update = new PacienteCreateDto
        {
            Nome = "Paciente Atualizado",
            Email = "novoemail@test.com",
            Telefone = "11987654321",
            Endereco = "Rua Atualizada, 456"
        };

        bool updated = await service.UpdateAsync(created.Id, update);

        updated.Should().BeTrue();
    }

    [Fact]
    public async Task DeletarAsync_DeveExcluirPaciente()
    {
        ApplicationDbContext context = CreateContext();
        IPacienteService service = CreateService(context);

        PacienteCreateDto dto = CreateValidPacienteDto("11144477735", "Paciente 1");
        PacienteReadDto created = await service.CreateAsync(dto);

        await service.InativarAsync(created.Id);

        bool exists = await context.Pacientes.AnyAsync(p => p.Id == created.Id);
        exists.Should().BeTrue();
    }
}