using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Models;

namespace SGHSS.Api.Data;

[ExcludeFromCodeCoverage]
public static class DbInitializer
{
    public static async Task SeedAdminAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        bool adminExists = await context.Usuarios.AnyAsync(u => u.Role == Role.Administrador);
        if (adminExists)
        {
            return;
        }

        string senhaPadrao = "Admin@123"; 

        byte[] passwordHash;
        byte[] passwordSalt;
        CreatePasswordHash(senhaPadrao, out passwordHash, out passwordSalt);

        Usuario admin = new Usuario
        {
            Username = "admin",
            Email = "admin@sghss.com",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = Role.Administrador,
            Ativo = true,
            PacienteId = null,
            ProfissionalSaudeId = null
        };

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();
    }

    private static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using (HMACSHA512 hmac = new HMACSHA512())
        {
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
