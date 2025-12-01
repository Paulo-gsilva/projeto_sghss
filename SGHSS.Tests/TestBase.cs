using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SGHSS.Api.Data;

namespace SGHSS.Tests;

[ExcludeFromCodeCoverage]
public abstract class TestBase
{
    protected ApplicationDbContext CreateContext()
    {
        DbContextOptions<ApplicationDbContext> options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        ApplicationDbContext context = new ApplicationDbContext(options);
        return context;
    }

    protected IConfiguration CreateConfiguration()
    {
        Dictionary<string, string?> settings = new Dictionary<string, string?>
        {
            { "Jwt:Secret", "TEST_KEY_1234567890_TEST_KEY_1234567890" },
            { "Jwt:Issuer", "SGHSS" },
            { "Jwt:Audience", "SGHSSUsers" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return configuration;
    }
}