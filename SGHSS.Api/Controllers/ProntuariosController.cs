using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProntuariosController : ControllerBase
{
    private readonly IProntuarioService _service;

    private readonly IConsultaService _consultaService;

    public ProntuariosController(IProntuarioService service, IConsultaService consultaService)
    {
        _service = service;
        _consultaService = consultaService;
    }

    [HttpGet("consulta/{consultaId:int}")]
    [Authorize(Roles = "Administrador,ProfissionalSaude,Paciente")]
    public async Task<ActionResult<ProntuarioReadDto>> GetByConsulta(int consultaId)
    {
        ProntuarioReadDto? dto = await _service.GetByConsultaIdAsync(consultaId);

        if (User.IsInRole("Paciente"))
        {
            string? claimPacienteId = User.FindFirst("pacienteId")?.Value;

            ConsultaReadDto? consultaReadDto = await _consultaService.GetByIdAsync(dto!.ConsultaId);

            if (string.IsNullOrEmpty(claimPacienteId) || consultaReadDto == null || consultaReadDto.PacienteId.ToString() != claimPacienteId.ToString())
            {
                return Forbid();
            }
        }

        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrador,ProfissionalSaude,Paciente")]
    public async Task<ActionResult<ProntuarioReadDto>> Get(int id)
    {
        ProntuarioReadDto? dto = await _service.GetByIdAsync(id);

        if (User.IsInRole("Paciente"))
        {
            string? claimPacienteId = User.FindFirst("pacienteId")?.Value;

            ConsultaReadDto? consultaReadDto = await _consultaService.GetByIdAsync(dto!.ConsultaId);

            if (string.IsNullOrEmpty(claimPacienteId) || consultaReadDto == null || consultaReadDto.PacienteId.ToString() != claimPacienteId.ToString())
            {
                return Forbid();
            }
        }
        
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "ProfissionalSaude")]
    public async Task<ActionResult<ProntuarioReadDto>> AtualizarProntuario(ProntuarioCreateDto dto)
    {
        try
        {
            ProntuarioReadDto created = await _service.CreateOrUpdateByConsultaAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ProfissionalSaude")]
    public async Task<IActionResult> Delete(int id)
    {
        bool removed = await _service.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}
