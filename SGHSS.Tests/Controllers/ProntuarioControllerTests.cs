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
public class ProntuarioControllerTests
{
    private readonly Mock<IProntuarioService> _serviceMock;
    private readonly Mock<IConsultaService> _consultaServiceMock;
    private readonly ProntuariosController _controller;

    public ProntuarioControllerTests()
    {
        _serviceMock = new Mock<IProntuarioService>();
        _consultaServiceMock = new Mock<IConsultaService>();
        _controller = new ProntuariosController(_serviceMock.Object, _consultaServiceMock.Object);
    }

    [Fact]
    public async Task GetByConsultaId_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetByConsultaIdAsync(99)).ReturnsAsync((ProntuarioReadDto?)null);

        ActionResult<ProntuarioReadDto> result = await _controller.GetByConsulta(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldReturnCreatedOrOk()
    {
        ProntuarioCreateDto dto = new ProntuarioCreateDto { ConsultaId = 1, Diagnostico = "Dx" };
        ProntuarioReadDto created = new ProntuarioReadDto { Id = 10, ConsultaId = 1, Diagnostico = "Dx" };

        _serviceMock.Setup(s => s.CreateOrUpdateByConsultaAsync(dto)).ReturnsAsync(created);

        ActionResult<ProntuarioReadDto> result = await _controller.AtualizarProntuario(dto);
        ActionResult<ProntuarioReadDto> action = result;

        CreatedAtActionResult createdAt = action.Result as CreatedAtActionResult;
        createdAt.Should().NotBeNull();
        createdAt!.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        IActionResult result = await _controller.Delete(99);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenRemoved()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        IActionResult result = await _controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }
}