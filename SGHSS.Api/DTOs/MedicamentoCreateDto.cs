using System;

namespace SGHSS.Api.DTOs;

public class MedicamentoCreateDto
{
    public string NomeMedicamento { get; set; } = null!;

    public string Dosagem { get; set; } = null!;

    public string Frequencia { get; set; } = null!;
    
    public string Duracao { get; set; } = null!;
}