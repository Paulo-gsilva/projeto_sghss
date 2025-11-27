using System;
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
    public async Task<ActionResult<IEnumerable<ConsultaReadDto>>> GetAll()
    {
        IReadOnlyList<ConsultaReadDto> list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
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
    public async Task<ActionResult<ConsultaReadDto>> Agendar(ConsultaCreateDto dto)
    {
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
    public async Task<IActionResult> Cancelar(int id)
    {
        bool ok = await _service.CancelarAsync(id);
        if (!ok)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("{id:int}/concluir")]
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