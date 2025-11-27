using System;

namespace SGHSS.Api.DTOs;

public class InternacaoCreateDto
{
    public int PacienteId { get; set; }

    public int LeitoId { get; set; }

    public string? Motivo { get; set; }
}
