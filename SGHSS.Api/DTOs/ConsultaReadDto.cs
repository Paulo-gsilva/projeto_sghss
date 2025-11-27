using System;
using SGHSS.Api.Models;

namespace SGHSS.Api.DTOs;

public class ConsultaReadDto
{
    public int Id { get; set; }

    public DateTime DataHora { get; set; }

    public TipoConsulta Tipo { get; set; }

    public StatusConsulta Status { get; set; }
    
    public string? Motivo { get; set; }

    public int PacienteId { get; set; }

    public string? PacienteNome { get; set; }

    public int ProfissionalSaudeId { get; set; }

    public string? ProfissionalNome { get; set; }

    public ConsultaReadDto() { }
 
    public ConsultaReadDto(Consulta consulta)
    {
        Id = consulta.Id;
        DataHora = consulta.DataHora;
        Tipo = consulta.Tipo;
        Status = consulta.Status;
        Motivo = consulta.Motivo;
        PacienteId = consulta.PacienteId;
        PacienteNome = consulta.Paciente != null ? consulta.Paciente.Nome : null;
        ProfissionalSaudeId = consulta.ProfissionalSaudeId;
        ProfissionalNome = consulta.ProfissionalSaude != null ? consulta.ProfissionalSaude.Nome : null;
    }
}
