using System.Text.Json;
using Derby.Backend.Data;
using Derby.Backend.Data;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DerbyContext>(options =>
    options.UseNpgsql(connectionString));

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApi();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key no configurada");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
builder.Services.AddScoped<IEquipoService, EquipoService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<ICompeticionRepository, CompeticionRepository>();
builder.Services.AddScoped<IPartidoRepository, PartidoRepository>();
builder.Services.AddScoped<ICompeticionService, CompeticionService>();

builder.Services.AddScoped<IArbitroRepository, ArbitroRepository>();
builder.Services.AddScoped<IArbitroService, ArbitroService>();

builder.Services.AddScoped<IEventoPartidoRepository, EventoPartidoRepository>();
builder.Services.AddScoped<IEventoPartidoService, EventoPartidoService>();

builder.Services.AddScoped<ILigaRepository, LigaRepository>();
builder.Services.AddScoped<ILigaService, LigaService>();

builder.Services.AddScoped<IPartidoService, PartidoService>();

builder.Services.AddScoped<IJugadorRepository, JugadorRepository>();
builder.Services.AddScoped<IJugadorService, JugadorService>();

var app = builder.Build();

app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:5101");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DerbyContext>();
    var migrated = false;
    for (int i = 0; i < 10; i++)
    {
        try
        {
            await context.Database.MigrateAsync();
            await DataSeeder.SeedAsync(context);
            migrated = true;
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BD no lista, reintentando en 3s... ({i + 1}/10): {ex.Message}");
            await Task.Delay(3000);
        }
    }
    if (!migrated) Console.WriteLine("No se pudo conectar a la BD tras 10 intentos.");
}

app.UseCors("AllowAngular");

app.UseAuthentication();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();