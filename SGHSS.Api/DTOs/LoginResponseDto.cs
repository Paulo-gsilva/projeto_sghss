using System;

namespace SGHSS.Api.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    
    public UsuarioReadDto Usuario { get; set; } = null!;
}
