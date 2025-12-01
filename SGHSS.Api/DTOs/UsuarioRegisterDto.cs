using System;
using System.ComponentModel.DataAnnotations;
using SGHSS.Api.Models;
using SGHSS.Api.Validators;

namespace SGHSS.Api.DTOs;

public class UsuarioRegisterDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = null!;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [Password]
    public string Senha { get; set; } = null!;

    [Required]
    public Role Role { get; set; }

    public int? PacienteId { get; set; }
    
    public int? ProfissionalSaudeId { get; set; }
}