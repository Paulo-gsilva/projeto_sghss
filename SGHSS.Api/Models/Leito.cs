using System;

namespace SGHSS.Api.Models;

public class Leito
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Tipo { get; set; }

    public StatusLeito Status { get; set; } = StatusLeito.Livre;

    public int UnidadeHospitalarId { get; set; }
    
    public UnidadeHospitalar UnidadeHospitalar { get; set; } = null!;
}