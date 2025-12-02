using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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

    public ProntuarioControllerTests()
    {
        _serviceMock = new Mock<IProntuarioService>();
        _consultaServiceMock = new Mock<IConsultaService>();
    }
    
    private ProntuariosController CreateControllerWithUserClaims(IProntuarioService service, bool isPaciente, int? pacienteId = null)
    {
        ProntuariosController controller = new ProntuariosController(service, _consultaServiceMock.Object);

        ClaimsIdentity identity;

        if (isPaciente)
        {
            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Paciente"),
                new Claim("pacienteId", pacienteId?.ToString() ?? "1")
            }, "TestAuth");
        }
        else
        {
            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Administrador"),
            }, "TestAuth");
        }

        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

        DefaultHttpContext httpContext = new DefaultHttpContext();
        httpContext.User = principal;

        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        return controller;
    }
    
    [Fact]
    public async Task GetByConsultaId_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetByConsultaIdAsync(99)).ReturnsAsync((ProntuarioReadDto?)null);

        ProntuariosController controller = CreateControllerWithUserClaims(_serviceMock.Object, isPaciente: false);

        ActionResult<ProntuarioReadDto> result = await controller.GetByConsulta(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldReturnCreatedOrOk()
    {
        ProntuarioCreateDto dto = new ProntuarioCreateDto { ConsultaId = 1, Diagnostico = "Dx" };
        ProntuarioReadDto created = new ProntuarioReadDto { Id = 10, ConsultaId = 1, Diagnostico = "Dx" };

        _serviceMock.Setup(s => s.CreateOrUpdateByConsultaAsync(dto)).ReturnsAsync(created);

        ProntuariosController controller = CreateControllerWithUserClaims(_serviceMock.Object, isPaciente: true, pacienteId: 1);

        ActionResult<ProntuarioReadDto> result = await controller.AtualizarProntuario(dto);
        ActionResult<ProntuarioReadDto> action = result;

        CreatedAtActionResult createdAt = action.Result as CreatedAtActionResult;
        createdAt.Should().NotBeNull();
        createdAt!.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        ProntuariosController controller = CreateControllerWithUserClaims(_serviceMock.Object, isPaciente: true, pacienteId: 1);

        IActionResult result = await controller.Delete(99);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenRemoved()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        ProntuariosController controller = CreateControllerWithUserClaims(_serviceMock.Object, isPaciente: true, pacienteId: 1);

        IActionResult result = await controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }
}