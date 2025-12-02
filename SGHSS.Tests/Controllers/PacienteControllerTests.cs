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
public class PacienteControllerTests
{
    private readonly Mock<IPacienteService> _serviceMock;
    private readonly PacientesController _controller;

    public PacienteControllerTests()
    {
        _serviceMock = new Mock<IPacienteService>();
        _controller = new PacientesController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WithList()
    {
        IReadOnlyList<PacienteReadDto> data = new List<PacienteReadDto>
        {
            new PacienteReadDto { Id = 1, Nome = "João" }
        };

        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(data);

        ActionResult<IEnumerable<PacienteReadDto>> result = await _controller.GetAll();

        OkObjectResult ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((PacienteReadDto?)null);

        ActionResult<PacienteReadDto?> result = await _controller.Get(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        PacienteCreateDto dto = new PacienteCreateDto { Nome = "Teste", Email = "a@a.com" };
        PacienteReadDto created = new PacienteReadDto { Id = 10, Nome = "Teste" };

        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        ActionResult<PacienteReadDto> result = await _controller.Create(dto);

        CreatedAtActionResult createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        _serviceMock.Setup(s => s.InativarAsync(1)).ReturnsAsync(true);

        IActionResult result = await _controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }
}
