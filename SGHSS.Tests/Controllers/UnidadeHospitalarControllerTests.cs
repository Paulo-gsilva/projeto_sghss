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

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        UnidadeHospitalarReadDto dto = new UnidadeHospitalarReadDto { Id = 2, Nome = "UH2" };
        _mock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(dto);

        ActionResult<UnidadeHospitalarReadDto> result = await _controller.Get(2);

        OkObjectResult ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _mock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((UnidadeHospitalarReadDto?)null);

        ActionResult<UnidadeHospitalarReadDto> result = await _controller.Get(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenUpdated()
    {
        UnidadeHospitalarCreateDto updateDto = new UnidadeHospitalarCreateDto { Nome = "UH Atualizada", Endereco = "Rua X" };
        UnidadeHospitalarReadDto updated = new UnidadeHospitalarReadDto { Id = 5, Nome = "UH Atualizada" };

        _mock.Setup(s => s.UpdateAsync(5, updateDto)).ReturnsAsync(true);

        IActionResult result = await _controller.Update(5, updateDto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenMissing()
    {
        UnidadeHospitalarCreateDto updateDto = new UnidadeHospitalarCreateDto { Nome = "X" };
        _mock.Setup(s => s.UpdateAsync(999, updateDto)).ReturnsAsync(false);

        IActionResult result = await _controller.Update(999, updateDto);

        result.Should().BeOfType<NotFoundResult>();
    }
}
