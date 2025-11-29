using System;
using SGHSS.Api.Models;

namespace SGHSS.Api.DTOs;

public class UsuarioRegisterDto
{
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Senha { get; set; } = null!;

    public Role Role { get; set; }

    public int? PacienteId { get; set; }
    
    public int? ProfissionalSaudeId { get; set; }
}