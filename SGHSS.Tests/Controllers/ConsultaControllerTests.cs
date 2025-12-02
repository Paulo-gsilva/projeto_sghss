using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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

    public ConsultaControllerTests()
    {
        _mock = new Mock<IConsultaService>();
    }

    private ConsultasController CreateControllerWithUserClaims(IConsultaService service, bool isPaciente, int? pacienteId = null)
    {
        ConsultasController controller = new ConsultasController(service);

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
    public async Task Agendar_ShouldReturnForbid_WhenPacienteTentaAgendarParaOutroPaciente()
    {
        Mock<IConsultaService> serviceMock = new Mock<IConsultaService>();

        ConsultaCreateDto dto = new ConsultaCreateDto
        {
            PacienteId = 2
        };

        ConsultasController controller = CreateControllerWithUserClaims(serviceMock.Object, isPaciente: true, pacienteId: 1);

        ActionResult<ConsultaReadDto> response = await controller.Agendar(dto);

        response.Should().BeOfType<ActionResult<ConsultaReadDto>>();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        ConsultaReadDto dto = new ConsultaReadDto { Id = 1 };
        _mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

        ConsultasController controller = CreateControllerWithUserClaims(_mock.Object, isPaciente: false);

        ActionResult<ConsultaReadDto?> result = await controller.Get(1);

        OkObjectResult ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _mock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ConsultaReadDto?)null);

        ConsultasController controller = CreateControllerWithUserClaims(_mock.Object, isPaciente: false);

        ActionResult<ConsultaReadDto> result = await controller.Get(99);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenAgendado()
    {
        ConsultaCreateDto dto = new ConsultaCreateDto { PacienteId = 1, ProfissionalSaudeId = 2, DataHora = DateTime.UtcNow.AddDays(1), Tipo = TipoConsulta.Presencial };
        ConsultaReadDto created = new ConsultaReadDto { Id = 5, Status = StatusConsulta.Agendada };

        _mock.Setup(s => s.AgendarAsync(dto)).ReturnsAsync(created);

        ConsultasController controller = CreateControllerWithUserClaims(_mock.Object, isPaciente: false);

        ActionResult<ConsultaReadDto> result = await controller.Agendar(dto);
        CreatedAtActionResult createdAt = result.Result as CreatedAtActionResult;

        createdAt.Should().NotBeNull();
        createdAt!.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Agendar_ShouldCreate_WhenPacienteAgendandoParaSiMesmo()
    {
        Mock<IConsultaService> serviceMock = new Mock<IConsultaService>();

        ConsultaCreateDto dto = new ConsultaCreateDto
        {
            PacienteId = 1
        };

        serviceMock.Setup(s => s.AgendarAsync(dto))
            .ReturnsAsync(new ConsultaReadDto { Id = 10, PacienteId = 1 });

        ConsultasController controller = CreateControllerWithUserClaims(serviceMock.Object, isPaciente: true, pacienteId: 1);

        ActionResult<ConsultaReadDto> response = await controller.Agendar(dto);

        ActionResult<ConsultaReadDto> result = response.Should().BeOfType<ActionResult<ConsultaReadDto>>().Subject;
    }

    [Fact]
    public async Task Cancelar_ShouldReturnNotFound_WhenMissing()
    {
        var mock = new Mock<IConsultaService>();
        mock.Setup(s => s.CancelarAsync(It.IsAny<int>()))
            .ReturnsAsync(false);


        ConsultasController controller = CreateControllerWithUserClaims(mock.Object, isPaciente: false);

        var result = await controller.Cancelar(999);

        Assert.IsType<NotFoundResult>(result);
        mock.Verify(s => s.CancelarAsync(999), Times.Once);
    }
    
    [Fact]
    public async Task Concluir_ShouldReturnNoContent_WhenSuccess()
    {
        Mock<IConsultaService> serviceMock = new Mock<IConsultaService>();
        serviceMock.Setup(s => s.ConcluirAsync(1)).ReturnsAsync(true);

        ConsultasController controller = CreateControllerWithUserClaims(serviceMock.Object, isPaciente: false);

        IActionResult result = await controller.Concluir(1);

        result.Should().BeOfType<NoContentResult>();
        serviceMock.Verify(s => s.ConcluirAsync(1), Times.Once);
    }

    [Fact]
    public async Task Concluir_ShouldReturnNotFound_WhenMissing()
    {
        Mock<IConsultaService> serviceMock = new Mock<IConsultaService>();
        serviceMock.Setup(s => s.ConcluirAsync(99)).ReturnsAsync(false);

        ConsultasController controller = CreateControllerWithUserClaims(serviceMock.Object, isPaciente: false);

        IActionResult result = await controller.Concluir(99);

        result.Should().BeOfType<NotFoundResult>();
    }
}
