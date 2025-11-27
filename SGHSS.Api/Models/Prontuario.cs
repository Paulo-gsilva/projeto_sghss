using System;

namespace SGHSS.Api.Models;

public class Prontuario
{
    public int Id { get; set; }

    public DateTime DataRegistro { get; set; } = DateTime.UtcNow;

    public string? Anotacoes { get; set; }

    public string? Diagnostico { get; set; }

    public string? PlanoTratamento { get; set; }

    public int ConsultaId { get; set; }
    
    public Consulta Consulta { get; set; } = null!;
}