using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services;
using SGHSS.Api.Services.Interfaces;
using Xunit;

namespace SGHSS.Tests.Services;

[ExcludeFromCodeCoverage]
public class UsuarioServiceTests : TestBase
{
    private IUsuarioService CreateService(ApplicationDbContext context)
    {
        IConfiguration configuration = CreateConfiguration();
        IUsuarioService service = new UsuarioService(context, configuration);
        return service;
    }

    [Fact]
    public async Task RegistrarAsync_DeveCriarUsuarioAdmin()
    {
        ApplicationDbContext context = CreateContext();
        IUsuarioService service = CreateService(context);

        UsuarioRegisterDto dto = new UsuarioRegisterDto
        {
            Username = "admin",
            Email = "admin@test.com",
            Senha = "Aa1!aaaa",
            Role = Role.Administrador
        };

        UsuarioReadDto result = await service.RegistrarAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Role.Should().Be(Role.Administrador);

        bool exists = await context.Usuarios.AnyAsync(u => u.Username == "admin");
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task RegistrarAsync_DeveLancarExcecao_QuandoEmailDuplicado()
    {
        ApplicationDbContext context = CreateContext();
        IUsuarioService service = CreateService(context);

        UsuarioRegisterDto dto1 = new UsuarioRegisterDto
        {
            Username = "user",
            Email = "user1@test.com",
            Senha = "Aa1!aaaa",
            Role = Role.Administrador
        };

        await service.RegistrarAsync(dto1);

        UsuarioRegisterDto dto2 = new UsuarioRegisterDto
        {
            Username = "user",
            Email = "user1@test.com",
            Senha = "Aa1!aaaa",
            Role = Role.Administrador
        };

        Func<Task> act = async () => await service.RegistrarAsync(dto2);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task LoginAsync_DeveRetornarToken_QuandoCredenciaisCorretas()
    {
        ApplicationDbContext context = CreateContext();
        IUsuarioService service = CreateService(context);

        UsuarioRegisterDto register = new UsuarioRegisterDto
        {
            Username = "loginuser",
            Email = "teste@gmail.com",
            Senha = "Aa1!aaaa",
            Role = Role.Administrador
        };

        await service.RegistrarAsync(register);

        LoginRequestDto login = new LoginRequestDto
        {
            Email = "teste@gmail.com",
            Senha = "Aa1!aaaa"
        };

        LoginResponseDto? response = await service.LoginAsync(login);

        response.Should().NotBeNull();
        response!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_DeveRetornarNull_QuandoSenhaIncorreta()
    {
        ApplicationDbContext context = CreateContext();
        IUsuarioService service = CreateService(context);

        UsuarioRegisterDto register = new UsuarioRegisterDto
        {
            Username = "loginuser",
            Email = "teste@gmail.com",
            Senha = "Aa1!aaaa",
            Role = Role.Administrador
        };

        await service.RegistrarAsync(register);

        LoginRequestDto login = new LoginRequestDto
        {
            Email = "teste@gmail.com",
            Senha = "123"
        };

        LoginResponseDto? response = await service.LoginAsync(login);

        response.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarListaDeUsuarios()
    {
        ApplicationDbContext context = CreateContext();
        IUsuarioService service = CreateService(context);

        UsuarioRegisterDto dto1 = new UsuarioRegisterDto
        {
            Username = "user1",
            Email = "user1@test.com",
            Senha = "Aa1!aaaa",
            Role = Role.Administrador
        };

        UsuarioRegisterDto dto2 = new UsuarioRegisterDto
        {
            Username = "user2",
            Email = "user2@test.com",
            Senha = "Aa1!aaaa",
            Role = Role.Administrador
        };

        await service.RegistrarAsync(dto1);
        await service.RegistrarAsync(dto2);

        IReadOnlyList<UsuarioReadDto> list = await service.GetAllAsync();

        list.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarUsuario_QuandoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IUsuarioService service = CreateService(context);

        UsuarioRegisterDto dto = new UsuarioRegisterDto
        {
            Username = "user",
            Email = "user@test.com",
            Senha = "Aa1!aaaa",
            Role = Role.Administrador
        };

        UsuarioReadDto created = await service.RegistrarAsync(dto);

        UsuarioReadDto? found = await service.GetByIdAsync(created.Id);

        found.Should().NotBeNull();
        found!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoNaoExiste()
    {
        ApplicationDbContext context = CreateContext();
        IUsuarioService service = CreateService(context);

        UsuarioReadDto? found = await service.GetByIdAsync(999);

        found.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_DeveExcluirUsuario()
    {
        ApplicationDbContext context = CreateContext();
        IUsuarioService service = CreateService(context);

        UsuarioRegisterDto dto = new UsuarioRegisterDto
        {
            Username = "todelete",
            Email = "todelete@test.com",
            Senha = "Aa1!aaaa",
            Role = Role.Administrador
        };

        UsuarioReadDto created = await service.RegistrarAsync(dto);

        await service.InativarAsync(created.Id);

        bool exists = await context.Usuarios.AnyAsync(u => u.Id == created.Id);
        exists.Should().BeTrue();

        UsuarioReadDto? inactive = await service.GetByIdAsync(created.Id);

        inactive!.Ativo.Should().BeFalse();
    }
}