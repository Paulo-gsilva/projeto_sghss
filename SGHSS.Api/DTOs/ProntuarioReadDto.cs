using System;

namespace SGHSS.Api.DTOs;

public class ProntuarioReadDto
{
    public int Id { get; set; }

    public int ConsultaId { get; set; }

    public DateTime DataRegistro { get; set; }

    public string? Anotacoes { get; set; }

    public string? Diagnostico { get; set; }

    public string? PlanoTratamento { get; set; }
}
