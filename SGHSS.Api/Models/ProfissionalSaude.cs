using System;

namespace SGHSS.Api.Models;

public class ProfissionalSaude
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public string RegistroProfissional { get; set; } = null!;

    public string Especialidade { get; set; } = null!;

    public string Email { get; set; } = null!;
 
    public string Telefone { get; set; } = null!;

    public bool Ativo { get; set; } = true;

    public ICollection<Consulta>? Consultas { get; set; }

    public int? UnidadeHospitalarId { get; set; }

    public UnidadeHospitalar? UnidadeHospitalar { get; set; }

    public Usuario? Usuario { get; set; }
}