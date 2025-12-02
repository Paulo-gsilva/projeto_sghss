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
public class ProfissionalSaudeControllerTests
{
    private readonly Mock<IProfissionalSaudeService> _serviceMock;
    private readonly ProfissionaisSaudeController _controller;

    public ProfissionalSaudeControllerTests()
    {
        _serviceMock = new Mock<IProfissionalSaudeService>();
        _controller = new ProfissionaisSaudeController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        IReadOnlyList<ProfissionalSaudeReadDto> list =
            new List<ProfissionalSaudeReadDto> { new ProfissionalSaudeReadDto { Id = 1, Nome = "Dr Teste" } };

        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

        ActionResult<IEnumerable<ProfissionalSaudeReadDto>> result = await _controller.GetAll();

        OkObjectResult ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(50)).ReturnsAsync((ProfissionalSaudeReadDto?)null);

        ActionResult<ProfissionalSaudeReadDto> result = await _controller.Get(50);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        ProfissionalSaudeCreateDto dto = new ProfissionalSaudeCreateDto { Nome = "Dr Y", Cpf = "11144477735", RegistroProfissional = "CRM1", Especialidade = "Clinico" };
        ProfissionalSaudeReadDto created = new ProfissionalSaudeReadDto { Id = 10, Nome = "Dr Y" };

        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        ActionResult<ProfissionalSaudeReadDto> result = await _controller.Create(dto);
        CreatedAtActionResult createdAt = result.Result as CreatedAtActionResult;

        createdAt.Should().NotBeNull();
        createdAt!.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenUpdated()
    {
        ProfissionalSaudeCreateDto dto = new ProfissionalSaudeCreateDto { Nome = "Dr Update", Especialidade = "Cardio" };

        _serviceMock.Setup(s => s.UpdateAsync(7, dto)).ReturnsAsync(true);

        IActionResult result = await _controller.Update(7, dto);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenMissing()
    {
        ProfissionalSaudeCreateDto dto = new ProfissionalSaudeCreateDto { Nome = "X" };
        _serviceMock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(false);

        IActionResult result = await _controller.Update(999, dto);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleted()
    {
        _serviceMock.Setup(s => s.InativarAsync(5)).ReturnsAsync(true);

        IActionResult result = await _controller.Delete(5);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.InativarAsync(999)).ReturnsAsync(false);

        IActionResult result = await _controller.Delete(999);

        result.Should().BeOfType<NotFoundResult>();
    }

}
