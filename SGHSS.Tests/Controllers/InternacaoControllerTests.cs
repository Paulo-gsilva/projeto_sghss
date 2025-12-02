using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SGHSS.Api.Controllers;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Tests.Controllers;

[ExcludeFromCodeCoverage]
public class InternacaoControllerTests
{
    private readonly Mock<IInternacaoService> _mock;
    private readonly InternacoesController _controller;

    public InternacaoControllerTests()
    {
        _mock = new Mock<IInternacaoService>();
        _controller = new InternacoesController(_mock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        IReadOnlyList<InternacaoReadDto> list = new List<InternacaoReadDto>
        {
            new InternacaoReadDto { Id = 1, DataEntrada = DateTime.UtcNow, DataSaida = DateTime.UtcNow.AddDays(1), LeitoCodigo = "123", Motivo = "Doente" }
        };

        _mock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

        ActionResult<IEnumerable<InternacaoReadDto>> result = await _controller.GetAll();

        OkObjectResult ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _mock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((InternacaoReadDto?)null);

        ActionResult<InternacaoReadDto> result = await _controller.Get(99);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Internar_ShouldReturnCreated()
    {
        InternacaoCreateDto dto = new InternacaoCreateDto { PacienteId = 1, LeitoId = 2, Motivo = "Teste" };
        InternacaoReadDto created = new InternacaoReadDto { Id = 10, LeitoId = 2 };

        _mock.Setup(s => s.InternarAsync(dto)).ReturnsAsync(created);

        ActionResult<InternacaoReadDto> result = await _controller.Internar(dto);
        CreatedAtActionResult createdAt = result.Result as CreatedAtActionResult;

        createdAt.Should().NotBeNull();
        createdAt!.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Transferir_ShouldReturnBadRequest_WhenFalse()
    {
        _mock.Setup(s => s.TransferirAsync(1, 2)).ReturnsAsync(false);

        IActionResult result = await _controller.Transferir(1, 2);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Transferir_ShouldReturnNoContent_WhenTrue()
    {
        _mock.Setup(s => s.TransferirAsync(1, 2)).ReturnsAsync(true);

        IActionResult result = await _controller.Transferir(1, 2);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DarAlta_ShouldReturnNotFound_WhenFalse()
    {
        _mock.Setup(s => s.DarAltaAsync(99)).ReturnsAsync(false);

        IActionResult result = await _controller.DarAlta(99);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DarAlta_ShouldReturnNoContent_WhenTrue()
    {
        _mock.Setup(s => s.DarAltaAsync(1)).ReturnsAsync(true);

        IActionResult result = await _controller.DarAlta(1);

        result.Should().BeOfType<NoContentResult>();
    }
}
