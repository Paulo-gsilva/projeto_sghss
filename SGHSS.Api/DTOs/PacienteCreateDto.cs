using System;
using System.ComponentModel.DataAnnotations;
using SGHSS.Api.Validators;

namespace SGHSS.Api.DTOs;

public class PacienteCreateDto
{   
    [Required]
    [StringLength(100, MinimumLength = 10)]
    public string Nome { get; set; } = null!;

    [Required]
    [Cpf]
    public string Cpf { get; set; } = null!;

    public DateTime DataNascimento { get; set; }

    [Required]
    [EmailAddress]    
    public string Email { get; set; } = null!;

    [RegularExpression(@"^(\+55\s?)?(\(?\d{2}\)?\s?)?(9?\d{4})-?\d{4}$", ErrorMessage = "Telefone em formato inválido.")]
    public string Telefone { get; set; } = null!;

    public string Endereco { get; set; } = null!;
}