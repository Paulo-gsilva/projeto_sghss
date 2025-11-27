using System;
using BCrypt.Net;

namespace SGHSS.Api.Models;

public class Usuario
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string SenhaHash { get; set; } = null!;

    public PerfilUsuario Perfil { get; set; }

    public int? PacienteId { get; set; }

    public Paciente? Paciente { get; set; }

    public int? ProfissionalSaudeId { get; set;}

    public ProfissionalSaude? ProfissionalSaude { get; set; }

    public bool ValidarSenha(string pwd) => BCrypt.Net.BCrypt.Verify(pwd, SenhaHash);

    public void AlterarSenha(string pwd)
    {
        SenhaHash = BCrypt.Net.BCrypt.HashPassword(pwd);
    }
}