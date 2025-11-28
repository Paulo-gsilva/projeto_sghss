using System;

namespace SGHSS.Api.DTOs;

public class MedicamentoReadDto
{
    public int Id { get; set; }

    public string NomeMedicamento { get; set; } = null!;

    public string Dosagem { get; set; } = null!;

    public string Frequencia { get; set; } = null!;
    
    public string Duracao { get; set; } = null!;
}