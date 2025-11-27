using System;
using SGHSS.Api.Models;

namespace SGHSS.Api.DTOs;

public class ProfissionalSaudeReadDto
{
    public int Id { get; set; }
    
    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public string RegistroProfissional { get; set; } = null!;

    public string Especialidade { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public bool Ativo { get; set; }

    public int? UnidadeHospitalarId { get; set; }

    public string? UnidadeHospitalarNome { get; set; }

    public ProfissionalSaudeReadDto() { }

    public ProfissionalSaudeReadDto(ProfissionalSaude profissionalSaude)
    {
        Id = profissionalSaude.Id;
        Nome = profissionalSaude.Nome;
        Cpf = profissionalSaude.Cpf;
        RegistroProfissional = profissionalSaude.RegistroProfissional;
        Especialidade = profissionalSaude.Especialidade;
        Email = profissionalSaude.Email;
        Telefone = profissionalSaude.Telefone;
        Ativo = profissionalSaude.Ativo;
        UnidadeHospitalarId = profissionalSaude.UnidadeHospitalarId;
        UnidadeHospitalarNome = profissionalSaude.UnidadeHospitalar != null ? profissionalSaude.UnidadeHospitalar.Nome : null;
    }
}