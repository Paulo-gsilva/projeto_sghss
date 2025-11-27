using System;

namespace SGHSS.Api.Models;

public class UnidadeHospitalar
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Endereco { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public ICollection<Leito>? Leitos { get; set; }

    public ICollection<ProfissionalSaude>? Profissionais { get; set; }
}