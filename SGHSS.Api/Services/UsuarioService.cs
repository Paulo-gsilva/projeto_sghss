using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGHSS.Api.Data;
using SGHSS.Api.DTOs;
using SGHSS.Api.Models;
using SGHSS.Api.Services.Interfaces;

namespace SGHSS.Api.Services;

public class UsuarioService: IUsuarioService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public UsuarioService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<UsuarioReadDto> RegistrarAsync(UsuarioRegisterDto dto)
    {
        bool usernameExists = await _context.Usuarios.AnyAsync(u => u.Username == dto.Username);
        if (usernameExists)
        {
            throw new InvalidOperationException("Username já está em uso.");
        }

        bool emailExists = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
        {
            throw new InvalidOperationException("Email já está em uso.");
        }

        // valida ligações
        if (dto.Role == Role.Paciente && dto.PacienteId == null)
        {
            throw new InvalidOperationException("Para role Paciente é necessário vincular um Paciente.");
        }

        if (dto.Role == Role.ProfissionalSaude && dto.ProfissionalSaudeId == null)
        {
            throw new InvalidOperationException("Para role ProfissionalSaude é necessário vincular um Profissional.");
        }

        Paciente? paciente = null;
        ProfissionalSaude? profissional = null;

        if (dto.PacienteId.HasValue)
        {
            paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.Id == dto.PacienteId.Value);
            if (paciente == null)
            {
                throw new InvalidOperationException("Paciente não encontrado.");
            }
        }

        if (dto.ProfissionalSaudeId.HasValue)
        {
            profissional = await _context.ProfissionaisSaude.FirstOrDefaultAsync(p => p.Id == dto.ProfissionalSaudeId.Value);
            if (profissional == null)
            {
                throw new InvalidOperationException("Profissional de saúde não encontrado.");
            }
        }

        byte[] passwordHash;
        byte[] passwordSalt;
        CreatePasswordHash(dto.Senha, out passwordHash, out passwordSalt);

        Usuario usuario = new Usuario
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = dto.Role,
            Ativo = true,
            Paciente = paciente,
            ProfissionalSaude = profissional
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        UsuarioReadDto result = MapToReadDto(usuario);
        return result;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        Usuario? usuario = await _context.Usuarios
            .Include(u => u.Paciente)
            .Include(u => u.ProfissionalSaude)
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.Ativo);

        if (usuario == null)
        {
            return null;
        }

        bool senhaOk = VerifyPasswordHash(dto.Senha, usuario.PasswordHash, usuario.PasswordSalt);
        if (!senhaOk)
        {
            return null;
        }

        string token = GenerateJwtToken(usuario);
        UsuarioReadDto usuarioDto = MapToReadDto(usuario);

        LoginResponseDto response = new LoginResponseDto
        {
            Token = token,
            Usuario = usuarioDto
        };

        return response;
    }

    public async Task<IReadOnlyList<UsuarioReadDto>> GetAllAsync()
    {
        List<Usuario> usuarios = await _context.Usuarios
            .AsNoTracking()
            .ToListAsync();

        List<UsuarioReadDto> list = usuarios
            .Select(u => MapToReadDto(u))
            .ToList();

        return list;
    }

    public async Task<UsuarioReadDto?> GetByIdAsync(int id)
    {
        Usuario? usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return null;
        }

        UsuarioReadDto dto = MapToReadDto(usuario);
        return dto;
    }

    public async Task<bool> InativarAsync(int id)
    {
        Usuario? usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return false;
        }

        usuario.Ativo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using (HMACSHA512 hmac = new HMACSHA512())
        {
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        using (HMACSHA512 hmac = new HMACSHA512(storedSalt))
        {
            byte[] computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            if (computed.Length != storedHash.Length)
            {
                return false;
            }

            for (int i = 0; i < computed.Length; i++)
            {
                if (computed[i] != storedHash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        string secret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret não configurado.");
        string issuer = _configuration["Jwt:Issuer"] ?? "SGHSS";
        string audience = _configuration["Jwt:Audience"] ?? "SGHSS-Clients";

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Username),
            new Claim("userid", usuario.Id.ToString()),
            new Claim(ClaimTypes.Role, usuario.Role.ToString()),
            new Claim("role", usuario.Role.ToString()),
            new Claim("pacienteId", usuario.PacienteId.HasValue ? usuario.PacienteId.Value.ToString() : string.Empty),
            new Claim("profissionalId", usuario.ProfissionalSaudeId.HasValue ? usuario.ProfissionalSaudeId.Value.ToString() : string.Empty)
        };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }

    private static UsuarioReadDto MapToReadDto(Usuario usuario)
    {
        UsuarioReadDto dto = new UsuarioReadDto
        {
            Id = usuario.Id,
            Username = usuario.Username,
            Email = usuario.Email,
            Role = usuario.Role,
            Ativo = usuario.Ativo,
            PacienteId = usuario.PacienteId,
            ProfissionalSaudeId = usuario.ProfissionalSaudeId
        };

        return dto;
    }
}
