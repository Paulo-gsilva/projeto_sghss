using System;
using SGHSS.Api.Models;

namespace SGHSS.Api.DTOs;

public class LeitoReadDto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Tipo { get; set; }

    public StatusLeito Status { get; set; }

    public int UnidadeHospitalarId { get; set; }
    
    public string? UnidadeHospitalarNome { get; set; }

    public LeitoReadDto() { }

    public LeitoReadDto(Leito leito)
    {
        Id = leito.Id;
        Codigo = leito.Codigo;
        Tipo = leito.Tipo;
        Status = leito.Status;
        UnidadeHospitalarId = leito.UnidadeHospitalarId;
        UnidadeHospitalarNome = leito.UnidadeHospitalar != null ? leito.UnidadeHospitalar.Nome : null;
    }
}