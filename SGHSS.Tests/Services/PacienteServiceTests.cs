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
public class PacienteServiceTests
{
    private ApplicationDbContext CreateContext()
    {
        DbContextOptions<ApplicationDbContext> options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        ApplicationDbContext context = new ApplicationDbContext(options);
        return context;
    }

    [Fact]
    public async Task CriarAsync_ShouldCreatePaciente()
    {
        ApplicationDbContext context = CreateContext();
        IPacienteService service = new PacienteService(context);

        PacienteCreateDto dto = new PacienteCreateDto
        {
            Nome = "João da Silva",
            Cpf = "11144477735",
            DataNascimento = new DateTime(1990, 1, 1),
            Email = "joao@test.com",
            Telefone = "11912345678",
            Endereco = "Rua A, 123"
        };

        PacienteReadDto result = await service.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Nome.Should().Be("João da Silva");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        ApplicationDbContext context = CreateContext();
        IPacienteService service = new PacienteService(context);

        PacienteReadDto? result = await service.GetByIdAsync(999);

        result.Should().BeNull();
    }
}