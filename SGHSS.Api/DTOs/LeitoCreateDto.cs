using System;

namespace SGHSS.Api.DTOs;

public class LeitoCreateDto
{
    public string Codigo { get; set; } = null!;

    public string? Tipo { get; set; }
    
    public int UnidadeHospitalarId { get; set; }
}