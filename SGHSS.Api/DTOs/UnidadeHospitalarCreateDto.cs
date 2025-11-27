using System;

namespace SGHSS.Api.DTOs;

public class UnidadeHospitalarCreateDto
{
    public string Nome { get; set; } = null!;

    public string Endereco { get; set; } = null!;
    
    public string Tipo { get; set; } = null!;
}