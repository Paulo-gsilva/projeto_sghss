using System;
using SGHSS.Api.Models;

namespace SGHSS.Api.DTOs;

public class UnidadeHospitalarReadDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string?Endereco { get; set; } = null!;

    public string Tipo { get; set; } = null!;
    
    public UnidadeHospitalarReadDto() { }

    public UnidadeHospitalarReadDto(UnidadeHospitalar unidade)
    {
        Id = unidade.Id;
        Nome = unidade.Nome;
        Endereco = unidade.Endereco;
        Tipo = unidade.Tipo;
    }
}