using System;
using Microsoft.AspNetCore.Mvc;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProntuariosController : ControllerBase
{
    private readonly IProntuarioService _service;

    public ProntuariosController(IProntuarioService service)
    {
        _service = service;
    }

    [HttpGet("consulta/{consultaId:int}")]
    public async Task<ActionResult<ProntuarioReadDto>> GetByConsulta(int consultaId)
    {
        ProntuarioReadDto? dto = await _service.GetByConsultaIdAsync(consultaId);
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProntuarioReadDto>> Get(int id)
    {
        ProntuarioReadDto? dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpPost]
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
