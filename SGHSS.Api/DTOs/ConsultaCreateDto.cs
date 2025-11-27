using System;
using SGHSS.Api.Models;

namespace SGHSS.Api.DTOs;

public class ConsultaCreateDto
{
    public int PacienteId { get; set; }

    public int ProfissionalSaudeId { get; set; }

    public DateTime DataHora { get; set; }

    public TipoConsulta Tipo { get; set; }
    
    public string? Motivo { get; set; }
}