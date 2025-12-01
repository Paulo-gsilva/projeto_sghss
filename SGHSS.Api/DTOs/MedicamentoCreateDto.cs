using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSS.Api.DTOs;

public class MedicamentoCreateDto
{
    [Required]
    public string NomeMedicamento { get; set; } = null!;

    [Required]
    public string Dosagem { get; set; } = null!;

    [Required]
    public string Frequencia { get; set; } = null!;
    
    [Required]
    public string Duracao { get; set; } = null!;
}