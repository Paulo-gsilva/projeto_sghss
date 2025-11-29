using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly IConsultaService _service;

    public ConsultasController(IConsultaService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,ProfissionalSaude")]
    public async Task<ActionResult<IEnumerable<ConsultaReadDto>>> GetAll()
    {
        IReadOnlyList<ConsultaReadDto> list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrador,ProfissionalSaude")]
    public async Task<ActionResult<ConsultaReadDto>> Get(int id)
    {
        ConsultaReadDto? dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Paciente")]
    public async Task<ActionResult<ConsultaReadDto>> Agendar(ConsultaCreateDto dto)
    {
        if (User.IsInRole("Paciente"))
        {
            string? claimPacienteId = User.FindFirst("pacienteId")?.Value;
            if (string.IsNullOrEmpty(claimPacienteId) || claimPacienteId != dto.PacienteId.ToString())
            {
                return Forbid();
            }
        }

        try
        {
            ConsultaReadDto created = await _service.AgendarAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:int}/cancelar")]
    [Authorize(Roles = "Administrador,Paciente")]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (User.IsInRole("Paciente"))
        {
            string? claimPacienteId = User.FindFirst("pacienteId")?.Value;
            if (string.IsNullOrEmpty(claimPacienteId) || claimPacienteId != id.ToString())
            {
                return Forbid();
            }
        }

        bool ok = await _service.CancelarAsync(id);
        if (!ok)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("{id:int}/concluir")]
    [Authorize(Roles = "ProfissionalSaude")]
    public async Task<IActionResult> Concluir(int id)
    {
        bool ok = await _service.ConcluirAsync(id);
        if (!ok)
        {
            return NotFound();
        }

        return NoContent();
    }
}