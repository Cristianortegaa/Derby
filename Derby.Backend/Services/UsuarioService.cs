﻿using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using CSharpFunctionalExtensions;
using System.Security.Cryptography;
using System.Text;

namespace Derby.Backend.Services;

public interface IUsuarioService
{
    Task<Result<UsuarioResponseDto, DerbyError>> RegistrarAsync(RegistroRequestDto dto);
    Task<Result<UsuarioResponseDto, DerbyError>> LoginAsync(UsuarioRequestDto dto);
    Task<Result<List<UsuarioResponseDto>, DerbyError>> ObtenerTodosAsync();
    Task<Result<UsuarioResponseDto, DerbyError>> ObtenerPorIdAsync(int id);
    Task<Result<UsuarioResponseDto, DerbyError>> ActualizarAsync(int id, UsuarioRequestDto dto);
    Task<Result<bool, DerbyError>> EliminarAsync(int id);
}

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(IUsuarioRepository repository, ILogger<UsuarioService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<UsuarioResponseDto, DerbyError>> RegistrarAsync(RegistroRequestDto dto)
    {
        _logger.LogInformation("Registrando nuevo usuario: {Email}", dto.Email);

        // Validar que el email no exista
        var emailExiste = await _repository.EmailExisteAsync(dto.Email);
        if (emailExiste)
        {
            _logger.LogWarning("Intento de registro con email existente: {Email}", dto.Email);
            return Result.Failure<UsuarioResponseDto, DerbyError>(
                new BadRequestError("Email ya registrado"));
        }

        // Crear el usuario
        var usuario = new Usuario
        {
            Email = dto.Email,
            Contraseña = HashPassword(dto.Contrasena),
            Rol = DeterminarRol(dto.Rol)
        };

        var usuarioCreado = await _repository.CrearAsync(usuario);

        _logger.LogInformation("Usuario registrado exitosamente: {Email}", dto.Email);

        return Result.Success<UsuarioResponseDto, DerbyError>(MapToDto(usuarioCreado));
    }

    public async Task<Result<UsuarioResponseDto, DerbyError>> LoginAsync(UsuarioRequestDto dto)
    {
        _logger.LogInformation("Intento de login: {Email}", dto.Email);

        // Buscar usuario por email
        var usuario = await _repository.ObtenerPorEmailAsync(dto.Email);
        if (usuario == null)
        {
            _logger.LogWarning("Login fallido: usuario no encontrado {Email}", dto.Email);
            return Result.Failure<UsuarioResponseDto, DerbyError>(
                new UnauthorizedError("Credenciales inválidas"));
        }

        // Verificar contraseña

        if (!VerifyPassword(dto.Contrasena, usuario.Contraseña))
        {
            _logger.LogWarning("Login fallido: contraseña incorrecta para {Email}", dto.Email);
            return Result.Failure<UsuarioResponseDto, DerbyError>(
                new UnauthorizedError("Credenciales inválidas"));
        }

        _logger.LogInformation("Login exitoso: {Email}", dto.Email);

        return Result.Success<UsuarioResponseDto, DerbyError>(MapToDto(usuario));
    }

    public async Task<Result<List<UsuarioResponseDto>, DerbyError>> ObtenerTodosAsync()
    {
        try
        {
            var usuarios = await _repository.ObtenerTodosAsync();
            var usuariosDto = usuarios.Select(MapToDto).ToList();
            return Result.Success<List<UsuarioResponseDto>, DerbyError>(usuariosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return Result.Failure<List<UsuarioResponseDto>, DerbyError>(
                new InternalServerError("Error al obtener usuarios"));
        }
    }

    public async Task<Result<UsuarioResponseDto, DerbyError>> ObtenerPorIdAsync(int id)
    {
        var usuario = await _repository.ObtenerPorIdAsync(id);
        if (usuario == null)
        {
            _logger.LogWarning("Usuario no encontrado: {Id}", id);
            return Result.Failure<UsuarioResponseDto, DerbyError>(
                new NotFoundError("Usuario no encontrado"));
        }

        return Result.Success<UsuarioResponseDto, DerbyError>(MapToDto(usuario));
    }

    public async Task<Result<UsuarioResponseDto, DerbyError>> ActualizarAsync(int id, UsuarioRequestDto dto)
    {
        _logger.LogInformation("Actualizando usuario: {Id}", id);

        var usuario = await _repository.ObtenerPorIdAsync(id);
        if (usuario == null)
        {
            _logger.LogWarning("Usuario no encontrado para actualizar: {Id}", id);
            return Result.Failure<UsuarioResponseDto, DerbyError>(
                new NotFoundError("Usuario no encontrado"));
        }

        // Si el email cambió, verificar que no esté en uso
        if (usuario.Email != dto.Email)
        {
            var emailExiste = await _repository.EmailExisteAsync(dto.Email);
            if (emailExiste)
            {
                _logger.LogWarning("Email ya registrado: {Email}", dto.Email);
                return Result.Failure<UsuarioResponseDto, DerbyError>(
                    new BadRequestError("Email ya registrado"));
            }

            usuario.Email = dto.Email;
        }

        // Actualizar contraseña si se proporciona
        if (!string.IsNullOrEmpty(dto.Contrasena))
        {
            usuario.Contraseña = HashPassword(dto.Contrasena);
        }

        usuario.Rol = DeterminarRol(dto.Rol);

        var usuarioActualizado = await _repository.ActualizarAsync(usuario);
        _logger.LogInformation("Usuario actualizado exitosamente: {Id}", id);

        return Result.Success<UsuarioResponseDto, DerbyError>(MapToDto(usuarioActualizado));
    }

    public async Task<Result<bool, DerbyError>> EliminarAsync(int id)
    {
        _logger.LogInformation("Eliminando usuario: {Id}", id);

        var usuario = await _repository.ObtenerPorIdAsync(id);
        if (usuario == null)
        {
            _logger.LogWarning("Usuario no encontrado para eliminar: {Id}", id);
            return Result.Failure<bool, DerbyError>(
                new NotFoundError("Usuario no encontrado"));
        }

        await _repository.EliminarAsync(id);
        _logger.LogInformation("Usuario eliminado exitosamente: {Id}", id);

        return Result.Success<bool, DerbyError>(true);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }

    private Rol DeterminarRol(string rol)
    {
        return rol.ToLower() switch
        {
            "administrador" => Rol.Administrador,
            "arbitro" => Rol.Arbitro,
            _ => Rol.Aficionado
        };
    }

    private UsuarioResponseDto MapToDto(Usuario usuario)
    {
        return new UsuarioResponseDto
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            Token = GenerarTokenSimple(usuario.Id)
        };
    }

    private string GenerarTokenSimple(int usuarioId)
    {
        // Token simple basado en ID y timestamp
        var data = $"{usuarioId}-{DateTime.UtcNow.Ticks}";
        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }
}

