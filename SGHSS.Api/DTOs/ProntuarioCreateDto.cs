using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSS.Api.DTOs;

public class ProntuarioCreateDto
{
    [Required]
    public int ConsultaId { get; set; }
    
    public string? Anotacoes { get; set; }

    public string? Diagnostico { get; set; }

    public string? PlanoTratamento { get; set; }
}
