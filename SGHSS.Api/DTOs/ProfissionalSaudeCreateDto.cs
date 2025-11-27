using System;

namespace SGHSS.Api.DTOs;

public class ProfissionalSaudeCreateDto
{
    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public string RegistroProfissional { get; set; } = null!;

    public string Especialidade { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public int? UnidadeHospitalarId { get; set; }
}