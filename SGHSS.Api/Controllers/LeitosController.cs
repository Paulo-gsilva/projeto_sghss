using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeitosController : ControllerBase
{
    private readonly ILeitoService _service;

    public LeitosController(ILeitoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeitoReadDto>>> GetAll()
    {
        IReadOnlyList<LeitoReadDto> list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeitoReadDto>> Get(int id)
    {
        LeitoReadDto? dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<LeitoReadDto>> Create(LeitoCreateDto dto)
    {
        LeitoReadDto created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, LeitoCreateDto dto)
    {
        bool updated = await _service.UpdateAsync(id, dto);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("{id:int}/status/{status:int}")]
    public async Task<IActionResult> AlterarStatus(int id, int status)
    {
        try
        {
            bool ok = await _service.AlterarStatusAsync(id, status);
            if (!ok)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (System.ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}