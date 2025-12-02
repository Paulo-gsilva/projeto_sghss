using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SGHSS.Api.Controllers;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Tests.Controllers;

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
}
