using System;

namespace SGHSS.Api.DTOs;

public class ReceitaReadDto
{
    public int Id { get; set; }

    public int ConsultaId { get; set; }

    public DateTime DataEmissao { get; set; }

    public string? Observacoes { get; set; }

    public string? CodigoValidacao { get; set; }

    public DateTime ValidaAte { get; set; }
    
    public List<MedicamentoReadDto> Medicamentos { get; set; } = new List<MedicamentoReadDto>();
}
