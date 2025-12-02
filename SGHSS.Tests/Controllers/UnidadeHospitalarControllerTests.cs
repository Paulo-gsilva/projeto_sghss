using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SGHSS.Api.Controllers;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Tests.Controllers;

public class UnidadeHospitalarControllerTests
{
    private readonly Mock<IUnidadeHospitalarService> _mock;
    private readonly UnidadesHospitalaresController _controller;

    public UnidadeHospitalarControllerTests()
    {
        _mock = new Mock<IUnidadeHospitalarService>();
        _controller = new UnidadesHospitalaresController(_mock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        IReadOnlyList<UnidadeHospitalarReadDto> list =
            new List<UnidadeHospitalarReadDto> { new UnidadeHospitalarReadDto { Id = 1, Nome = "Unidade Central" } };

        _mock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

        ActionResult<IEnumerable<UnidadeHospitalarReadDto>> result = await _controller.GetAll();

        OkObjectResult ok = result.Result as OkObjectResult;

        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        UnidadeHospitalarCreateDto dto = new UnidadeHospitalarCreateDto { Nome = "UH Nova", Endereco = "Rua" };
        UnidadeHospitalarReadDto created = new UnidadeHospitalarReadDto { Id = 5, Nome = "UH Nova" };

        _mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        ActionResult<UnidadeHospitalarReadDto> result = await _controller.Create(dto);
        CreatedAtActionResult createdAt = result.Result as CreatedAtActionResult;

        createdAt.Should().NotBeNull();
        createdAt!.Value.Should().BeEquivalentTo(created);
    }
}
