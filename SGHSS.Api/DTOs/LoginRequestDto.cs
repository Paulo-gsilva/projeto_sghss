using System;

namespace SGHSS.Api.DTOs;

public class LoginRequestDto
{
    public string Email { get; set; } = null!;
    
    public string Senha { get; set; } = null!;
}
