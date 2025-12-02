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
public class UsuarioControllerTests
{
    private readonly Mock<IUsuarioService> _mock;
    private readonly AuthController _controller;

    public UsuarioControllerTests()
    {
        _mock = new Mock<IUsuarioService>();
        _controller = new AuthController(_mock.Object);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalid()
    {
        LoginRequestDto login = new LoginRequestDto { Email = "x", Senha = "y" };
        _mock.Setup(s => s.LoginAsync(login)).ReturnsAsync((LoginResponseDto?)null);

        ActionResult<LoginResponseDto> result = await _controller.Login(login);
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenValid()
    {
        LoginRequestDto login = new LoginRequestDto { Email = "u", Senha = "p" };
        LoginResponseDto response = new LoginResponseDto { Token = "t", Usuario = new UsuarioReadDto { Id = 1, Username = "u" } };

        _mock.Setup(s => s.LoginAsync(login)).ReturnsAsync(response);

        ActionResult<LoginResponseDto> result = await _controller.Login(login);
        OkObjectResult ok = result.Result as OkObjectResult;

        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        UsuarioRegisterDto r = new UsuarioRegisterDto { Username = "x", Email = "e", Senha = "Senha1!", Role = SGHSS.Api.Models.Role.Administrador };
        _mock.Setup(s => s.RegistrarAsync(r)).ThrowsAsync(new System.InvalidOperationException("Erro"));

        ActionResult<UsuarioReadDto> result = await _controller.Register(r);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetUsuarios_ShouldReturnList_WhenAdmin()
    {
        IReadOnlyList<UsuarioReadDto> list = new List<UsuarioReadDto> { new UsuarioReadDto { Id = 1, Username = "admin" } };
        _mock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

        ActionResult<IEnumerable<UsuarioReadDto>> result = await _controller.GetUsuarios();
        OkObjectResult ok = result.Result as OkObjectResult;

        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetUsuario_ShouldReturnOk_WhenFound()
    {
        UsuarioReadDto usuario = new UsuarioReadDto { Id = 1, Username = "admin" };
        _mock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(usuario);

        ActionResult<UsuarioReadDto> result = await _controller.GetUsuario(1);
        OkObjectResult ok = result.Result as OkObjectResult;

        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(usuario);
    }

    [Fact]
    public async Task GetUsuario_ShouldReturnNotFound_WhenMissing()
    {
        _mock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((UsuarioReadDto?)null);

        ActionResult<UsuarioReadDto> result = await _controller.GetUsuario(99);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task InativarUsuario_ShouldReturnNoContent_WhenSuccess()
    {
        _mock.Setup(s => s.InativarAsync(1)).ReturnsAsync(true);

        IActionResult result = await _controller.InativarUsuario(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task InativarUsuario_ShouldReturnNotFound_WhenMissing()
    {
        _mock.Setup(s => s.InativarAsync(99)).ReturnsAsync(false);

        IActionResult result = await _controller.InativarUsuario(99);

        result.Should().BeOfType<NotFoundResult>();
    }
}