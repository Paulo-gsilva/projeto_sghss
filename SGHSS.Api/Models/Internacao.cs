using System;

namespace SGHSS.Api.Models;

public class Internacao
{
    public int Id { get; set; }

    public DateTime DataEntrada { get; set; } = DateTime.UtcNow;

    public DateTime? DataSaida { get; set; }
    
    public string? Motivo { get; set; }

    public StatusInternacao Status { get; set; }

    public int PacienteId { get; set; }

    public Paciente Paciente { get; set; } = null!;

    public int LeitoId { get; set; }
    
    public Leito Leito { get; set; } = null!;
}