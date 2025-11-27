using System;

namespace SGHSS.Api.Models;

public class ReceitaDigital
{
    public int Id { get; set; }

    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;

    public string? Observacoes { get; set; }
    
    public string? CodigoValidacao { get; set; }
    
    public DateTime ValidaAte { get; set; }

    public ICollection<MedicamentoPrescrito>? Medicamentos { get; set; }
}