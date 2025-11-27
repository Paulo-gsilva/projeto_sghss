using System;
using SGHSS.Api.Models;

namespace SGHSS.Api.DTOs;

public class InternacaoReadDto
{
    public int Id { get; set; }

    public DateTime DataEntrada { get; set; }

    public DateTime? DataSaida { get; set; }

    public string? Motivo { get; set; }

    public StatusInternacao Status { get; set; }

    public int PacienteId { get; set; }
    
    public string? PacienteNome { get; set; }

    public int LeitoId { get; set; }

    public string? LeitoCodigo { get; set; }

    public InternacaoReadDto() { }

    public InternacaoReadDto(Internacao internacao)
    {
        Id = internacao.Id;
        DataEntrada = internacao.DataEntrada;
        DataSaida = internacao.DataSaida;
        Motivo = internacao.Motivo;
        Status = internacao.Status;
        PacienteId = internacao.PacienteId;
        PacienteNome = internacao.Paciente != null ? internacao.Paciente.Nome : null;
        LeitoId = internacao.LeitoId;
        LeitoCodigo = internacao.Leito != null ? internacao.Leito.Codigo : null;
    }
}