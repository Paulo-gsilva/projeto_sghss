using System;

namespace SGHSS.Api.DTOs;

public class ProntuarioCreateDto
{
    public int ConsultaId { get; set; }
    
    public string? Anotacoes { get; set; }

    public string? Diagnostico { get; set; }

    public string? PlanoTratamento { get; set; }
}
