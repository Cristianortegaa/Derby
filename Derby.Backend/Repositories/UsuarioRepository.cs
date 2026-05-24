﻿using Derby.Backend.Data;
using Derby.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly DerbyContext _context;

    public UsuarioRepository(DerbyContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email)
    {
        return await _context.Usuarios.Include(u => u.Arbitro).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        return await _context.Usuarios.Include(u => u.Arbitro).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario> CrearAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<Usuario> ActualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task EliminarAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
