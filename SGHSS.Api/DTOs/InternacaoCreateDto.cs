using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSS.Api.DTOs;

public class InternacaoCreateDto
{
    [Required]
    public int PacienteId { get; set; }

    [Required]
    public int LeitoId { get; set; }

    public string? Motivo { get; set; }
}
