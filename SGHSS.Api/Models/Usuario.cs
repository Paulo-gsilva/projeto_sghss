using System;
using BCrypt.Net;

namespace SGHSS.Api.Models;

public class Usuario
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    public Role Role { get; set; }

    public bool Ativo { get; set; } = true;

    public int? PacienteId { get; set; }

    public Paciente? Paciente { get; set; }

    public int? ProfissionalSaudeId { get; set; }
    
    public ProfissionalSaude? ProfissionalSaude { get; set; }
}