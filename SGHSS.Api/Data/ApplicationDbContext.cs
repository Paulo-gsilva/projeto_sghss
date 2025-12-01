using System;
using Microsoft.EntityFrameworkCore;
using SGHSS.Api.Models;

namespace SGHSS.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Paciente> Pacientes => Set<Paciente>();

    public DbSet<ProfissionalSaude> ProfissionaisSaude => Set<ProfissionalSaude>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Consulta> Consultas => Set<Consulta>();

    public DbSet<Prontuario> Prontuarios => Set<Prontuario>();

    public DbSet<ReceitaDigital> ReceitasDigitais => Set<ReceitaDigital>();

    public DbSet<MedicamentoPrescrito> MedicamentosPrescritos => Set<MedicamentoPrescrito>();

    public DbSet<UnidadeHospitalar> UnidadesHospitalares => Set<UnidadeHospitalar>();

    public DbSet<Leito> Leitos => Set<Leito>();

    public DbSet<Internacao> Internacoes => Set<Internacao>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<Usuario>()
            .HasOne(u => u.Paciente)
            .WithOne(p => p.Usuario)
            .HasForeignKey<Usuario>(u => u.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Usuario>()
            .HasOne(u => u.ProfissionalSaude)
            .WithOne(p => p.Usuario)
            .HasForeignKey<Usuario>(u => u.ProfissionalSaudeId)
            .OnDelete(DeleteBehavior.Restrict);

        // UnidadeHospitalar 1 - N Leito
        builder.Entity<UnidadeHospitalar>()
            .HasMany(u => u.Leitos)
            .WithOne(l => l.UnidadeHospitalar)
            .HasForeignKey(l => l.UnidadeHospitalarId);

        // UnidadeHospitalar 1 - N ProfissionalSaude
        builder.Entity<UnidadeHospitalar>()
            .HasMany(u => u.Profissionais)
            .WithOne(p => p.UnidadeHospitalar)
            .HasForeignKey(p => p.UnidadeHospitalarId)
            .IsRequired(false);

        // Paciente 1 - N Consulta
        builder.Entity<Paciente>()
            .HasMany(p => p.Consultas)
            .WithOne(c => c.Paciente)
            .HasForeignKey(c => c.PacienteId);

        // ProfissionalSaude 1 - N Consulta
        builder.Entity<ProfissionalSaude>()
            .HasMany(p => p.Consultas)
            .WithOne(c => c.ProfissionalSaude)
            .HasForeignKey(c => c.ProfissionalSaudeId);

        // Consulta 1 - 1 Prontuario
        builder.Entity<Consulta>()
            .HasOne(c => c.Prontuario)
            .WithOne(p => p.Consulta)
            .HasForeignKey<Prontuario>(p => p.ConsultaId)
            .IsRequired(false);

        // Consulta - ReceitaDigital (0..1)
        builder.Entity<Consulta>()
            .HasOne(c => c.ReceitaDigital)
            .WithMany()
            .HasForeignKey(c => c.ReceitaDigitalId)
            .IsRequired(false);

        // ReceitaDigital 1 - N MedicamentoPrescrito
        builder.Entity<ReceitaDigital>()
            .HasMany(r => r.Medicamentos)
            .WithOne(m => m.ReceitaDigital)
            .HasForeignKey(m => m.ReceitaDigitalId);
    }
}