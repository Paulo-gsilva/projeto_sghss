using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SGHSS.Api.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SGHSS.Api.Filters;
using SGHSS.Api.Services;
using SGHSS.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IProfissionalSaudeService, ProfissionalSaudeService>();
builder.Services.AddScoped<IUnidadeHospitalarService, UnidadeHospitalarService>();
builder.Services.AddScoped<ILeitoService, LeitoService>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();
builder.Services.AddScoped<IInternacaoService, InternacaoService>();
builder.Services.AddScoped<IProntuarioService, ProntuarioService>();
builder.Services.AddScoped<IReceitaService, ReceitaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// JWT Auth
string? jwtSecret = builder.Configuration["Jwt:Secret"];
string? jwtIssuer = builder.Configuration["Jwt:Issuer"];
string? jwtAudience = builder.Configuration["Jwt:Audience"];

SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization();

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<ResponseWrapperFilter>();
        options.Filters.Add<ExceptionWrapperFilter>();
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SGHSS API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT no formato: Bearer {seu token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",         
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();