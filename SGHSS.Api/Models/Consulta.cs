using System;

namespace SGHSS.Api.Models;

public class Consulta
{
    public int Id { get; set; }

    public DateTime DataHora { get; set; }

    public TipoConsulta Tipo { get; set; }

    public StatusConsulta Status { get; set; }

    public string? Motivo { get; set; }

    public int PacienteId { get; set; }

    public Paciente Paciente { get; set; } = null!;

    public int ProfissionalSaudeId { get; set; }

    public ProfissionalSaude ProfissionalSaude { get; set; } = null!;

    public int? ProntuarioId { get; set; }

    public Prontuario? Prontuario { get; set; }

    public int? ReceitaDigitalId { get; set; }
    
    public ReceitaDigital? ReceitaDigital { get; set; }
}