using System;

namespace SGHSS.Api.Models;

public class MedicamentoPrescrito
{
    public int Id { get; set; }

    public string NomeMedicamento { get; set; } = null!;

    public string Dosagem { get; set; } = null!;

    public string Frequencia { get; set; } = null!;

    public string Duracao { get; set; } = null!;

    public int ReceitaDigitalId { get; set; }
    
    public ReceitaDigital ReceitaDigital { get; set; } = null!;
}