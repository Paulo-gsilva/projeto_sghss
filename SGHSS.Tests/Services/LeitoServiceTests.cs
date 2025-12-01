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
public class LeitoServiceTests: TestBase
{
    private ILeitoService CreateService(ApplicationDbContext context)
    {
        ILeitoService service = new LeitoService(context);
        return service;
    }

    private async Task<int> SeedUnidadeAsync(ApplicationDbContext context, string nome = "Hospital Teste")
    {
        UnidadeHospitalar unidade = new UnidadeHospitalar
        {
            Nome = nome,
            Endereco = "Rua Teste, 100",
            Tipo = "Hospital"
        };
        context.UnidadesHospitalares.Add(unidade);
        await context.SaveChangesAsync();
        return unidade.Id;
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoLeitos()
    {
        ApplicationDbContext context = CreateContext();
        ILeitoService service = CreateService(context);

        IReadOnlyList<LeitoReadDto> result = await service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnLeitosWithUnidadeInfo()
    {
        ApplicationDbContext context = CreateContext();
        int unidadeId = await SeedUnidadeAsync(context, "Central Hospital");

        Leito leito = new Leito
        {
            Codigo = "A-101",
            Tipo = "UTI",
            UnidadeHospitalarId = unidadeId,
            Status = StatusLeito.Livre
        };
        context.Leitos.Add(leito);
        await context.SaveChangesAsync();

        ILeitoService service = CreateService(context);

        IReadOnlyList<LeitoReadDto> result = await service.GetAllAsync();

        result.Should().HaveCount(1);
        result[0].Codigo.Should().Be("A-101");
        result[0].UnidadeHospitalarNome.Should().Be("Central Hospital");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        ApplicationDbContext context = CreateContext();
        ILeitoService service = CreateService(context);

        LeitoReadDto? result = await service.GetByIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnLeito_WhenExists()
    {
        ApplicationDbContext context = CreateContext();
        int unidadeId = await SeedUnidadeAsync(context, "Unidade Norte");

        Leito leito = new Leito
        {
            Codigo = "B-202",
            Tipo = "Enfermaria",
            UnidadeHospitalarId = unidadeId,
            Status = StatusLeito.Livre
        };
        context.Leitos.Add(leito);
        await context.SaveChangesAsync();

        ILeitoService service = CreateService(context);

        LeitoReadDto? result = await service.GetByIdAsync(leito.Id);

        result.Should().NotBeNull();
        result!.Codigo.Should().Be("B-202");
        result.UnidadeHospitalarNome.Should().Be("Unidade Norte");
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateLeito_WhenUnidadeExists()
    {
        ApplicationDbContext context = CreateContext();
        int unidadeId = await SeedUnidadeAsync(context);

        LeitoCreateDto dto = new LeitoCreateDto
        {
            Codigo = "C-303",
            Tipo = "Enfermaria",
            UnidadeHospitalarId = unidadeId
        };

        ILeitoService service = CreateService(context);

        LeitoReadDto created = await service.CreateAsync(dto);

        created.Should().NotBeNull();
        created.Codigo.Should().Be("C-303");

        Leito? persisted = await context.Leitos.FirstOrDefaultAsync(l => l.Id == created.Id);
        persisted.Should().NotBeNull();
        persisted!.Codigo.Should().Be("C-303");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateLeito_WhenExists()
    {
        ApplicationDbContext context = CreateContext();
        int unidadeId = await SeedUnidadeAsync(context);
        Leito leito = new Leito
        {
            Codigo = "D-404",
            Tipo = "Enfermaria",
            UnidadeHospitalarId = unidadeId,
            Status = StatusLeito.Livre
        };
        context.Leitos.Add(leito);
        await context.SaveChangesAsync();

        LeitoCreateDto updateDto = new LeitoCreateDto
        {
            Codigo = "D-405",
            Tipo = "UTI",
            UnidadeHospitalarId = unidadeId
        };

        ILeitoService service = CreateService(context);

        bool updated = await service.UpdateAsync(leito.Id, updateDto);

        updated.Should().BeTrue();

        Leito? persisted = await context.Leitos.FirstOrDefaultAsync(l => l.Id == leito.Id);
        persisted!.Codigo.Should().Be("D-405");
        persisted.Tipo.Should().Be("UTI");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenLeitoNotFound()
    {
        ApplicationDbContext context = CreateContext();

        LeitoCreateDto updateDto = new LeitoCreateDto
        {
            Codigo = "D-405",
            Tipo = "UTI",
            UnidadeHospitalarId = 1
        };

        ILeitoService service = CreateService(context);

        bool updated = await service.UpdateAsync(9999, updateDto);

        updated.Should().BeFalse();
    }
}