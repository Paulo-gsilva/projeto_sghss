using System;

namespace SGHSS.Api.DTOs;

public class ReceitaCreateDto
{
    public int ConsultaId { get; set; }
    
    public string? Observacoes { get; set; }

    public DateTime? ValidaAte { get; set; }

    public List<MedicamentoCreateDto> Medicamentos { get; set; } = new List<MedicamentoCreateDto>();
}