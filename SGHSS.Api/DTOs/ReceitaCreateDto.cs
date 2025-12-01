using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSS.Api.DTOs;

public class ReceitaCreateDto
{
    [Required]
    public int ConsultaId { get; set; }
    
    public string? Observacoes { get; set; }

    public DateTime? ValidaAte { get; set; }

    public List<MedicamentoCreateDto> Medicamentos { get; set; } = new List<MedicamentoCreateDto>();
}