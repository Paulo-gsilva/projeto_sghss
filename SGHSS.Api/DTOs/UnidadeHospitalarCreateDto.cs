using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSS.Api.DTOs;

public class UnidadeHospitalarCreateDto
{
    [Required]
    [StringLength(100, MinimumLength = 10)]
    public string Nome { get; set; } = null!;

    [Required]
    public string Endereco { get; set; } = null!;
    
    [Required]
    public string Tipo { get; set; } = null!;
}