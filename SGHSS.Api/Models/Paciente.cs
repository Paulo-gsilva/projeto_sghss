using System;

namespace SGHSS.Api.Models;

public class Paciente
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public DateTime DataNascimento { get; set; }

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Endereco { get; set; } = null!;

    public bool Ativo { get; set; } = true;

    public ICollection<Consulta>? Consultas { get; set; }

    public ICollection<Internacao>? Internacoes { get; set; }

    public Usuario? Usuario { get; set; }
}