using System;
using Microsoft.AspNetCore.Mvc;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InternacoesController : ControllerBase
{
    private readonly IInternacaoService _service;

    public InternacoesController(IInternacaoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InternacaoReadDto>>> GetAll()
    {
        IReadOnlyList<InternacaoReadDto> list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InternacaoReadDto>> Get(int id)
    {
        InternacaoReadDto? dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<InternacaoReadDto>> Internar(InternacaoCreateDto dto)
    {
        try
        {
            InternacaoReadDto created = await _service.InternarAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:int}/transferir/{novoLeitoId:int}")]
    public async Task<IActionResult> Transferir(int id, int novoLeitoId)
    {
        try
        {
            bool ok = await _service.TransferirAsync(id, novoLeitoId);
            if (!ok)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:int}/alta")]
    public async Task<IActionResult> DarAlta(int id)
    {
        bool ok = await _service.DarAltaAsync(id);
        if (!ok)
        {
            return NotFound();
        }

        return NoContent();
    }
}