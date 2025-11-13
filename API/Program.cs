using Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Interfaces;
using Application.Services;
using DotNetEnv;
using FluentValidation;

// Cargar variables de entorno desde .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);
// AGREGAR SERVICIOS DE OPENAPI
builder.Services.AddOpenApi();

// AGREGAR SERVICIOS DE AUTORIZACIÓN
builder.Services.AddAuthorization();

// AGREGAR CONTROLADORES (para endpoints)
builder.Services.AddControllers();

// CONFIGURAR BASE DE DATOS
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// CONFIGURAR IDENTITY (sistema de usuarios)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Políticas de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Configuración de usuarios
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";
    options.User.RequireUniqueEmail = true;

    // Configuración de lockout (bloqueo de cuenta)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Configuración de confirmación
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
//  TODO: investigar estas opciones
// .AddTokenProvider<EmailTokenProvider<ApplicationUser>>("email") // Token provider personalizado
// .AddPasswordValidator<CustomPasswordValidator<ApplicationUser>>(); // Validador personalizado

// REGISTRAR MEDIATR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly);
    cfg.AddOpenBehavior(typeof(Application.Common.Behaviors.ValidationBehavior<,>));
});

// REGISTRAR FLUENT VALIDATION
builder.Services.AddValidatorsFromAssembly(typeof(Application.AssemblyReference).Assembly);

// REGISTRAR SERVICIOS
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());
    
// CONFIGURAR JWT (autenticación)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validar la clave de firma (CRÍTICO)
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            // Validar el emisor del token
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            // Validar la audiencia del token
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            // Validar tiempo de vida
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Sin tolerancia para diferencias de reloj
        };
    });

var app = builder.Build();

// CONFIGURAR EL PIPELINE DE LA APLICACIÓN
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// MAPEAR CONTROLADORES
app.MapControllers();

app.Run();