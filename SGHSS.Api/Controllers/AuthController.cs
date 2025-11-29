
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSS.Api.DTOs;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public AuthController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost("register")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UsuarioReadDto>> Register(UsuarioRegisterDto dto)
    {
        try
        {
            UsuarioReadDto created = await _usuarioService.RegistrarAsync(dto);
            return CreatedAtAction(nameof(GetUsuario), new { id = created.Id }, created);
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
    {
        LoginResponseDto? response = await _usuarioService.LoginAsync(dto);
        if (response == null)
        {
            return Unauthorized("Credenciais inválidas.");
        }

        return Ok(response);
    }

    [HttpGet("usuarios")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<IEnumerable<UsuarioReadDto>>> GetUsuarios()
    {
        IReadOnlyList<UsuarioReadDto> list = await _usuarioService.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("usuarios/{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UsuarioReadDto>> GetUsuario(int id)
    {
        UsuarioReadDto? dto = await _usuarioService.GetByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpDelete("usuarios/{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> InativarUsuario(int id)
    {
        bool ok = await _usuarioService.InativarAsync(id);
        if (!ok)
        {
            return NotFound();
        }

        return NoContent();
    }
}