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
public class UnidadeHospitalarServiceTests: TestBase
{
    private IUnidadeHospitalarService CreateService(ApplicationDbContext context)
    {
        IUnidadeHospitalarService service = new UnidadeHospitalarService(context);
        return service;
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoUnidades()
    {
        ApplicationDbContext context = CreateContext();
        IUnidadeHospitalarService service = CreateService(context);

        IReadOnlyList<UnidadeHospitalarReadDto> list = await service.GetAllAsync();

        list.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUnidades()
    {
        ApplicationDbContext context = CreateContext();

        UnidadeHospitalar unidade = new UnidadeHospitalar
        {
            Nome = "Hospital Teste 1",
            Endereco = "Rua 1",
            Tipo = "Hospital"
        };
        context.UnidadesHospitalares.Add(unidade);
        await context.SaveChangesAsync();

        IUnidadeHospitalarService service = CreateService(context);

        IReadOnlyList<UnidadeHospitalarReadDto> list = await service.GetAllAsync();

        list.Should().HaveCount(1);
        list[0].Nome.Should().Be("Hospital Teste 1");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        ApplicationDbContext context = CreateContext();
        IUnidadeHospitalarService service = CreateService(context);

        UnidadeHospitalarReadDto? dto = await service.GetByIdAsync(9999);

        dto.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUnidade_WhenExists()
    {
        ApplicationDbContext context = CreateContext();

        UnidadeHospitalar unidade = new UnidadeHospitalar
        {
            Nome = "Hospital A",
            Endereco = "Av A",
            Tipo = "Hospital"
        };
        context.UnidadesHospitalares.Add(unidade);
        await context.SaveChangesAsync();

        IUnidadeHospitalarService service = CreateService(context);

        UnidadeHospitalarReadDto? dto = await service.GetByIdAsync(unidade.Id);

        dto.Should().NotBeNull();
        dto!.Nome.Should().Be("Hospital A");
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUnidade()
    {
        ApplicationDbContext context = CreateContext();
        IUnidadeHospitalarService service = CreateService(context);

        UnidadeHospitalarCreateDto dto = new UnidadeHospitalarCreateDto
        {
            Nome = "Hospital Criado",
            Endereco = "Rua X",
            Tipo = "Hospital"
        };

        UnidadeHospitalarReadDto created = await service.CreateAsync(dto);

        created.Should().NotBeNull();
        created.Nome.Should().Be("Hospital Criado");

        UnidadeHospitalar? persisted = await context.UnidadesHospitalares.FirstOrDefaultAsync(u => u.Id == created.Id);
        persisted.Should().NotBeNull();
        persisted!.Endereco.Should().Be("Rua X");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate_WhenExists()
    {
        ApplicationDbContext context = CreateContext();

        UnidadeHospitalar unidade = new UnidadeHospitalar
        {
            Nome = "Hospital Velho",
            Endereco = "Rua Velha",
            Tipo = "Hospital"
        };
        context.UnidadesHospitalares.Add(unidade);
        await context.SaveChangesAsync();

        UnidadeHospitalarCreateDto updateDto = new UnidadeHospitalarCreateDto
        {
            Nome = "X",
            Endereco = "Y"
        };

        IUnidadeHospitalarService service = CreateService(context);

        bool updated = await service.UpdateAsync(unidade.Id, updateDto);

        updated.Should().BeTrue();

        UnidadeHospitalar? persisted = await context.UnidadesHospitalares.FirstOrDefaultAsync(u => u.Id == unidade.Id);
        persisted!.Nome.Should().Be("X");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotFound()
    {
        ApplicationDbContext context = CreateContext();
        IUnidadeHospitalarService service = CreateService(context);

        UnidadeHospitalarCreateDto updateDto = new UnidadeHospitalarCreateDto
        {
            Nome = "X",
            Endereco = "Y"
        };

        bool updated = await service.UpdateAsync(9999, updateDto);

        updated.Should().BeFalse();
    }
}