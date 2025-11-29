using System;

namespace SGHSS.Api.DTOs;

public class LoginRequestDto
{
    public string Username { get; set; } = null!;
    
    public string Senha { get; set; } = null!;
}
