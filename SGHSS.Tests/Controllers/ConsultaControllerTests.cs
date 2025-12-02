using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SGHSS.Api.Controllers;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Tests.Controllers;

[ExcludeFromCodeCoverage]
public class ConsultaControllerTests
{
    private readonly Mock<IConsultaService> _mock;
    private readonly ConsultasController _controller;

    public ConsultaControllerTests()
    {
        _mock = new Mock<IConsultaService>();
        _controller = new ConsultasController(_mock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        ConsultaReadDto dto = new ConsultaReadDto { Id = 1 };
        _mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

        ActionResult<ConsultaReadDto?> result = await _controller.Get(1);

        OkObjectResult ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _mock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ConsultaReadDto?)null);

        ActionResult<ConsultaReadDto> result = await _controller.Get(99);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenAgendado()
    {
        ConsultaCreateDto dto = new ConsultaCreateDto { PacienteId = 1, ProfissionalSaudeId = 2, DataHora = DateTime.UtcNow.AddDays(1), Tipo = TipoConsulta.Presencial };
        ConsultaReadDto created = new ConsultaReadDto { Id = 5, Status = StatusConsulta.Agendada };

        _mock.Setup(s => s.AgendarAsync(dto)).ReturnsAsync(created);

        ActionResult<ConsultaReadDto> result = await _controller.Agendar(dto);
        CreatedAtActionResult createdAt = result.Result as CreatedAtActionResult;

        createdAt.Should().NotBeNull();
        createdAt!.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Cancelar_ShouldReturnNoContent_WhenSuccess()
    {
        _mock.Setup(s => s.CancelarAsync(1)).ReturnsAsync(true);

        IActionResult result = await _controller.Cancelar(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Cancelar_ShouldReturnNotFound_WhenMissing()
    {
        _mock.Setup(s => s.CancelarAsync(99)).ReturnsAsync(false);

        IActionResult result = await _controller.Cancelar(99);

        result.Should().BeOfType<NotFoundResult>();
    }
}
