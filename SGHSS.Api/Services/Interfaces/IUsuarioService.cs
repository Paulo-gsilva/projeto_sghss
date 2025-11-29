using System;
using SGHSS.Api.DTOs;

namespace SGHSS.Api.Services.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioReadDto> RegistrarAsync(UsuarioRegisterDto dto);

    Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);

    Task<IReadOnlyList<UsuarioReadDto>> GetAllAsync();

    Task<UsuarioReadDto?> GetByIdAsync(int id);
    
    Task<bool> InativarAsync(int id);
}
