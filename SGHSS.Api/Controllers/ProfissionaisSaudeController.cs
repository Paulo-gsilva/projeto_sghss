using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfissionaisSaudeController : ControllerBase
{
    private readonly IProfissionalSaudeService _service;

    public ProfissionaisSaudeController(IProfissionalSaudeService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,ProfissionalSaude")]
    public async Task<ActionResult<IEnumerable<ProfissionalSaudeReadDto>>> GetAll()
    {
        IReadOnlyList<ProfissionalSaudeReadDto> list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrador,ProfissionalSaude")]
    public async Task<ActionResult<ProfissionalSaudeReadDto>> Get(int id)
    {
        ProfissionalSaudeReadDto? dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProfissionalSaudeReadDto>> Create(ProfissionalSaudeCreateDto dto)
    {
        try
        {
            ProfissionalSaudeReadDto created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (System.InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, ProfissionalSaudeCreateDto dto)
    {
        bool updated = await _service.UpdateAsync(id, dto);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        bool inativado = await _service.InativarAsync(id);
        if (!inativado)
        {
            return NotFound();
        }

        return NoContent();
    }
}