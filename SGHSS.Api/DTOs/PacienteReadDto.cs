using System;
using SGHSS.Api.Models;

namespace SGHSS.Api.DTOs;

public class PacienteReadDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public DateTime DataNascimento { get; set; }

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Endereco { get; set; } = null!;
    
    public bool Ativo { get; set; }

    public PacienteReadDto() { }

    public PacienteReadDto(Paciente p)
    {
        Id = p.Id;
        Nome = p.Nome;
        Cpf = p.Cpf;
        DataNascimento = p.DataNascimento;
        Email = p.Email;
        Telefone = p.Telefone;
        Endereco = p.Endereco;
        Ativo = p.Ativo;
    }
}
