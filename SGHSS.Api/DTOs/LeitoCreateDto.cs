using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSS.Api.DTOs;

public class LeitoCreateDto
{
    [Required]
    public string Codigo { get; set; } = null!;

    public string? Tipo { get; set; }
    
    [Required]
    public int UnidadeHospitalarId { get; set; }
}