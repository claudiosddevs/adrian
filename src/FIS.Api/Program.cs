using System.Text;
using Asp.Versioning;
using FIS.Api.Identity;
using FIS.Api.Middleware;
using FIS.Application;
using FIS.Application.Common.Interfaces;
using FIS.Infrastructure;
using FIS.Infrastructure.Persistence;
using FIS.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ─── Logging (Serilog) ───────────────────────────────────────────────
builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// ─── Capas de la aplicación ──────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Pipeline de validación de MediatR
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ─── Autenticación JWT ───────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key no configurado.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "fis-api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "fis-clients";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// ─── Versionado de API ───────────────────────────────────────────────
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
}).AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
});

// ─── Controllers + Swagger ───────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Full Internet Services API",
        Version = "v1",
        Description = "API REST del sistema de monitoreo de soporte técnico y ventas."
    });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT con el prefijo 'Bearer '. Ej: 'Bearer eyJ...'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ─── CORS para clientes externos (Web + Desktop en dev) ──────────────
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p => p
    .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

var app = builder.Build();

// ─── Pipeline HTTP ───────────────────────────────────────────────────
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FIS API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Migración + seeding al arranque (solo Dev por defecto).
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<FisDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    try
    {
        await DataSeeder.SeedAsync(db, hasher);
        Log.Information("Migración y sembrado completados.");
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "No fue posible aplicar migraciones / seed (¿SQL Server disponible?).");
    }
}

app.Run();

public partial class Program { }
