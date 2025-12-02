using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SGHSS.Api.Controllers;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;
using SGHSS.Api.Models;

namespace SGHSS.Tests.Controllers;

[ExcludeFromCodeCoverage]
public class LeitoControllerTests
{
    private readonly Mock<ILeitoService> _mock;
    private readonly LeitosController _controller;

    public LeitoControllerTests()
    {
        _mock = new Mock<ILeitoService>();
        _controller = new LeitosController(_mock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        IReadOnlyList<LeitoReadDto> list = new List<LeitoReadDto>
        {
            new LeitoReadDto { Id = 1, Codigo = "101" }
        };

        _mock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

        ActionResult<IEnumerable<LeitoReadDto>> result = await _controller.GetAll();

        OkObjectResult ok = result.Result as OkObjectResult;

        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        LeitoCreateDto dto = new LeitoCreateDto { Codigo = "L100", UnidadeHospitalarId = 1 };
        LeitoReadDto created = new LeitoReadDto { Id = 10, Codigo = "L100" };

        _mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        ActionResult<LeitoReadDto> result = await _controller.Create(dto);
        CreatedAtActionResult createdAt = result.Result as CreatedAtActionResult;

        createdAt.Should().NotBeNull();
        createdAt!.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        LeitoReadDto dto = new LeitoReadDto { Id = 1, Codigo = "101", UnidadeHospitalarNome = "UH" };
        _mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

        ActionResult<LeitoReadDto> result = await _controller.Get(1);

        OkObjectResult ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _mock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((LeitoReadDto?)null);

        ActionResult<LeitoReadDto> result = await _controller.Get(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenUpdated()
    {
        LeitoCreateDto updateDto = new LeitoCreateDto { Codigo = "L200", Tipo = "UTI", UnidadeHospitalarId = 1 };

        _mock.Setup(s => s.UpdateAsync(5, updateDto)).ReturnsAsync(true);

        IActionResult result = await _controller.Update(5, updateDto);

        result.Should().NotBeNull();
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenMissing()
    {
        LeitoCreateDto updateDto = new LeitoCreateDto { Codigo = "X" };
        _mock.Setup(s => s.UpdateAsync(999, updateDto)).ReturnsAsync(false);

        IActionResult result = await _controller.Update(999, updateDto);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AlterarStatus_ShouldReturnNoContent_WhenSuccess()
    {
        _mock.Setup(s => s.AlterarStatusAsync(1, (int)StatusLeito.Ocupado)).ReturnsAsync(true);

        IActionResult result = await _controller.AlterarStatus(1, (int)StatusLeito.Ocupado);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AlterarStatus_ShouldReturnBadRequest_WhenFails()
    {
        _mock.Setup(s => s.AlterarStatusAsync(1, (int)StatusLeito.Livre)).ReturnsAsync(false);

        IActionResult result = await _controller.AlterarStatus(1, (int)StatusLeito.Livre);

        result.Should().BeOfType<NotFoundResult>();
    }

}
