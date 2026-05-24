using Derby.Backend.Models;

namespace Derby.Backend.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario> CrearAsync(Usuario usuario);
    Task<bool> EmailExisteAsync(string email);
    Task<List<Usuario>> ObtenerTodosAsync();
    Task<Usuario> ActualizarAsync(Usuario usuario);
    Task EliminarAsync(int id);
}
