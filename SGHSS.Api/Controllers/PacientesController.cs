using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _pacienteService;

    public PacientesController(IPacienteService pacienteService)
    {
        _pacienteService = pacienteService;
    }

    [HttpGet]   
    [Authorize(Roles = "Administrador,ProfissionalSaude")]
    public async Task<ActionResult<IEnumerable<PacienteReadDto>>> GetAll()
    {
        IReadOnlyList<PacienteReadDto> pacientes = await _pacienteService.GetAllAsync();
        
        return Ok(pacientes);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrador,ProfissionalSaude")]
    public async Task<ActionResult<PacienteReadDto>> Get(int id)
    {
        PacienteReadDto? paciente = await _pacienteService.GetByIdAsync(id);

        if (paciente == null)
        {
            return NotFound();
        }

        return Ok(paciente);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<PacienteReadDto>> Create(PacienteCreateDto dto)
    {
        try
        {
            PacienteReadDto created = await _pacienteService.CreateAsync(dto);

            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (System.InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, PacienteCreateDto dto)
    {
        bool updated = await _pacienteService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(Get), new { id = id }, updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        bool inativado = await _pacienteService.InativarAsync(id);

        if (!inativado)
        {
            return NotFound();
        }

        return NoContent();
    }
}