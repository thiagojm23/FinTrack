using System.Text;
using FinTrack.Application.Interfaces;
using FinTrack.Application.Services;
using FinTrack.Domain.Interfaces.Repositorios;
using FinTrack.Infrastructure.Configs;
using FinTrack.Infrastructure.Data;
using FinTrack.Infrastructure.Repositorios;
using FinTrack.Infrastructure.Servicos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configurar EmailSettings para Injeção de dependência
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register repositories
builder.Services.AddScoped<ITransacaoRepositorio, TransacaoRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IServicoAutenticacao, AutenticacaoServico>();
builder.Services.AddScoped<IServicoEmail, EmailServico>();
builder.Services.AddScoped<TransacaoApplicationServico>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Swagger para usar Autenticação JWT (Adicionado)
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "FinTrack API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, insira 'Bearer' seguido de um espaço e o token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey, // Alterado para ApiKey para formato Bearer simples
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Configure JWT Authentication (Adicionado)
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key não configurado");
var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Defina como true em produção se usar HTTPS
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true, // Valide se o emissor é quem você espera
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true, // Valide se o token é para sua aplicação
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true, // Verifique se o token não expirou
        ClockSkew = TimeSpan.Zero // Remove a tolerância padrão de 5 minutos na expiração
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowVueApp"); // Deve vir antes de UseAuthentication e UseAuthorization

// Adiciona Authorization policies se necessário (ex: roles)
app.UseAuthentication(); // Adicionado - Habilita autenticação
app.UseAuthorization(); // Adicionado - Habilita autorização

app.MapControllers();

// Ensure database is created with initial migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();