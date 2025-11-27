using System;

namespace SGHSS.Api.DTOs;

public class PacienteCreateDto
{
    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public DateTime DataNascimento { get; set; }

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Endereco { get; set; } = null!;
}