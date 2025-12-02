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
public class ReceitaDigitalControllerTests
    {
        private readonly Mock<IReceitaService> _serviceMock;
        private readonly Mock<IConsultaService> _consultaServiceMock;
        private readonly ReceitasController _controller;

        public ReceitaDigitalControllerTests()
        {
            _serviceMock = new Mock<IReceitaService>();
            _consultaServiceMock = new Mock<IConsultaService>();
            _controller = new ReceitasController(_serviceMock.Object, _consultaServiceMock.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenMissing()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ReceitaReadDto?)null);

            ActionResult<ReceitaReadDto> result = await _controller.Get(99);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            ReceitaCreateDto dto = new ReceitaCreateDto { ConsultaId = 1, Medicamentos = new List<MedicamentoCreateDto>() };
            ReceitaReadDto created = new ReceitaReadDto { Id = 2 };

            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            ActionResult<ReceitaReadDto> result = await _controller.Receitar(dto);
            CreatedAtActionResult createdAt = result.Result as CreatedAtActionResult;

            createdAt.Should().NotBeNull();
            createdAt!.Value.Should().BeEquivalentTo(created);
        }

        [Fact]
        public async Task AddMedicamento_ShouldReturnNotFound_WhenReceitaMissing()
        {
            MedicamentoCreateDto med = new MedicamentoCreateDto { NomeMedicamento = "X" };
            _serviceMock.Setup(s => s.AddMedicamentoAsync(99, med)).ReturnsAsync(false);

            IActionResult result = await _controller.AdicionarMedicamento(99, med);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddMedicamento_ShouldReturnNoContent_WhenAdded()
        {
            MedicamentoCreateDto med = new MedicamentoCreateDto { NomeMedicamento = "X" };
            _serviceMock.Setup(s => s.AddMedicamentoAsync(1, med)).ReturnsAsync(true);

            IActionResult result = await _controller.AdicionarMedicamento(1, med);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ValidarCodigo_ShouldReturnOk_WhenValid()
        {
            _serviceMock.Setup(s => s.ValidarCodigoAsync("CODEOK")).ReturnsAsync(true);

            ActionResult<bool> result = await _controller.ValidarCodigo("CODEOK");
            OkObjectResult ok = result.Result as OkObjectResult;

            ok.Should().NotBeNull();
        }

        [Fact]
        public async Task ValidarCodigo_ShouldReturnNotFound_WhenInvalid()
        {
            _serviceMock.Setup(s => s.ValidarCodigoAsync("NOK")).ReturnsAsync(false);

            ActionResult<bool> result = await _controller.ValidarCodigo("NOK");
            NotFoundObjectResult nf = result.Result as NotFoundObjectResult;

            nf.Should().NotBeNull();
            nf!.Value.Should().Be(false);
        }
    }
