using System;
using System.ComponentModel.DataAnnotations;
using SGHSS.Api.Validators;

namespace SGHSS.Api.DTOs;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    [Password]
    public string Senha { get; set; } = null!;
}
