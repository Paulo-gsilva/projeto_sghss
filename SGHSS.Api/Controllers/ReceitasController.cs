using System;
using Microsoft.AspNetCore.Mvc;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceitasController : ControllerBase
{
    private readonly IReceitaService _service;

    public ReceitasController(IReceitaService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReceitaReadDto>> Get(int id)
    {
        ReceitaReadDto? dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ReceitaReadDto>> Receitar(ReceitaCreateDto dto)
    {
        try
        {
            ReceitaReadDto created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{receitaId:int}/medicamentos")]
    public async Task<IActionResult> AdicionarMedicamento(int receitaId, MedicamentoCreateDto dto)
    {
        bool ok = await _service.AddMedicamentoAsync(receitaId, dto);
        if (!ok)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{receitaId:int}/medicamentos/{medicamentoId:int}")]
    public async Task<IActionResult> RemoverMedicamento(int receitaId, int medicamentoId)
    {
        bool ok = await _service.RemoveMedicamentoAsync(receitaId, medicamentoId);
        if (!ok)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("validar/{codigo}")]
    public async Task<ActionResult<bool>> ValidarCodigo(string codigo)
    {
        bool ok = await _service.ValidarCodigoAsync(codigo);
        return Ok(ok);
    }
}