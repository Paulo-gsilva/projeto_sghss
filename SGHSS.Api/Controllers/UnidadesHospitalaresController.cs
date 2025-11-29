using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UnidadesHospitalaresController : ControllerBase
{
    private readonly IUnidadeHospitalarService _service;

    public UnidadesHospitalaresController(IUnidadeHospitalarService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UnidadeHospitalarReadDto>>> GetAll()
    {
        IReadOnlyList<UnidadeHospitalarReadDto> list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<UnidadeHospitalarReadDto>> Get(int id)
    {
        UnidadeHospitalarReadDto? dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UnidadeHospitalarReadDto>> Create(UnidadeHospitalarCreateDto dto)
    {
        UnidadeHospitalarReadDto created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, UnidadeHospitalarCreateDto dto)
    {
        bool updated = await _service.UpdateAsync(id, dto);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
}
