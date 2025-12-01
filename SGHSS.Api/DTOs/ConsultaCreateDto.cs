using System;
using System.ComponentModel.DataAnnotations;
using SGHSS.Api.Models;

namespace SGHSS.Api.DTOs;

public class ConsultaCreateDto
{
    [Required]
    public int PacienteId { get; set; }

    [Required]
    public int ProfissionalSaudeId { get; set; }

    public DateTime DataHora { get; set; }

    public TipoConsulta Tipo { get; set; }
    
    public string? Motivo { get; set; }
}