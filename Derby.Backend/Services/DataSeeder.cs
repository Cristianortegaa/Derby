using Derby.Backend.Data;
using Derby.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Derby.Backend.Services;

public class DataSeeder
{
    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public static async Task SeedAsync(DerbyContext context)
    {
        // Crear usuarios si no existen
        var usuariosExisten = await context.Usuarios.AnyAsync();
        if (!usuariosExisten)
        {
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Email = "admin@derby.com",
                    Contraseña = HashPassword("Admin@123"),
                    Rol = Rol.Administrador
                },
                new Usuario
                {
                    Email = "arbitro@derby.com",
                    Contraseña = HashPassword("Arbitro@123"),
                    Rol = Rol.Arbitro
                }
            };

                await context.Usuarios.AddRangeAsync(usuarios);
                await context.SaveChangesAsync();
                Console.WriteLine("✓ Usuarios creados");
            }
            else
            {
                Console.WriteLine("✓ Usuarios ya existen");
            }

            // Crear equipos si no existen
            var equiposExisten = await context.Equipos.AnyAsync();
            if (!equiposExisten)
            {
                Console.WriteLine("• Creando equipos...");
            var equipos = new List<Equipo>
            {
                new Equipo 
                { 
                    Nombre = "Getafe CF", 
                    Sede = "Polideportivo Getafe", 
                    Division = "1", 
                    EscudoUrl = "https://via.placeholder.com/50?text=Getafe" 
                },
                new Equipo 
                { 
                    Nombre = "Rayo Vallecano", 
                    Sede = "Vallecas", 
                    Division = "1", 
                    EscudoUrl = "https://via.placeholder.com/50?text=Rayo" 
                },
                new Equipo 
                { 
                    Nombre = "Leganés B", 
                    Sede = "Polideportivo Butarque", 
                    Division = "2", 
                    EscudoUrl = "https://via.placeholder.com/50?text=Leganes" 
                },
                new Equipo 
                { 
                    Nombre = "Madrid CFF", 
                    Sede = "Antiguo Canódromo", 
                    Division = "fem", 
                    EscudoUrl = "https://via.placeholder.com/50?text=MadridCFF" 
                },
                new Equipo 
                { 
                    Nombre = "Alcalá CF", 
                    Sede = "Estadio Alcalá", 
                    Division = "1", 
                    EscudoUrl = "https://via.placeholder.com/50?text=Alcala" 
                },
                new Equipo 
                { 
                    Nombre = "Fuenlabrada", 
                    Sede = "Fernando Torres", 
                    Division = "1", 
                    EscudoUrl = "https://via.placeholder.com/50?text=Fuenlabrada" 
                }
            };

            await context.Equipos.AddRangeAsync(equipos);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Equipos creados");
        }
        else
        {
            Console.WriteLine("✓ Equipos ya existen");
        }

        // Crear competiciones si no existen
        var competicionesExisten = await context.Competiciones.AnyAsync();
        if (!competicionesExisten)
        {
            Console.WriteLine("• Creando competiciones...");
            var competiciones = new List<Competicion>
            {
                new Competicion 
                { 
                    Nombre = "Copa RFEF Fase Autonómica", 
                    Temporada = "2025-2026", 
                    TipoJuego = "futbol11", 
                    Grupo = "Grupo A" 
                },
                new Competicion 
                { 
                    Nombre = "Liga Nacional", 
                    Temporada = "2025-2026", 
                    TipoJuego = "futbol11", 
                    Grupo = "Grupo A" 
                },
                new Competicion 
                { 
                    Nombre = "Copa de Aficionados", 
                    Temporada = "2025-2026", 
                    TipoJuego = "futbol7", 
                    Grupo = "Grupo B" 
                }
            };

            await context.Competiciones.AddRangeAsync(competiciones);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Competiciones creadas");
        }
        else
        {
            Console.WriteLine("✓ Competiciones ya existen");
        }

        // Crear partidos si no existen
        var partidosExisten = await context.Partidos.AnyAsync();
        if (!partidosExisten)
        {
            Console.WriteLine("• Creando partidos...");
            var partidos = new List<Partido>
            {
                // Jornada 1
                new Partido 
                { 
                    Fecha = new DateTime(2026, 4, 25, 19, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = 2, 
                    GolesVisitantes = 1, 
                    Finalizado = true, 
                    Jornada = 1, 
                    CompeticionId = 1, 
                    EquipoLocalId = 1, 
                    EquipoVisitanteId = 2 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 4, 25, 20, 30, 0, DateTimeKind.Utc), 
                    GolesLocal = 1, 
                    GolesVisitantes = 1, 
                    Finalizado = true, 
                    Jornada = 1, 
                    CompeticionId = 1, 
                    EquipoLocalId = 3, 
                    EquipoVisitanteId = 4 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 4, 26, 19, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = 3, 
                    GolesVisitantes = 0, 
                    Finalizado = true, 
                    Jornada = 1, 
                    CompeticionId = 1, 
                    EquipoLocalId = 5, 
                    EquipoVisitanteId = 6 
                },

                // Jornada 2
                new Partido 
                { 
                    Fecha = new DateTime(2026, 5, 2, 19, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = 0, 
                    GolesVisitantes = 2, 
                    Finalizado = true, 
                    Jornada = 2, 
                    CompeticionId = 1, 
                    EquipoLocalId = 2, 
                    EquipoVisitanteId = 3 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 5, 2, 20, 30, 0, DateTimeKind.Utc), 
                    GolesLocal = 1, 
                    GolesVisitantes = 0, 
                    Finalizado = true, 
                    Jornada = 2, 
                    CompeticionId = 1, 
                    EquipoLocalId = 4, 
                    EquipoVisitanteId = 5 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 5, 3, 19, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = 2, 
                    GolesVisitantes = 2, 
                    Finalizado = true, 
                    Jornada = 2, 
                    CompeticionId = 1, 
                    EquipoLocalId = 6, 
                    EquipoVisitanteId = 1 
                },

                // Jornada 3 (Pendientes)
                new Partido 
                { 
                    Fecha = new DateTime(2026, 5, 9, 19, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = null, 
                    GolesVisitantes = null, 
                    Finalizado = false, 
                    Jornada = 3, 
                    CompeticionId = 1, 
                    EquipoLocalId = 1, 
                    EquipoVisitanteId = 3 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 5, 9, 20, 30, 0, DateTimeKind.Utc), 
                    GolesLocal = null, 
                    GolesVisitantes = null, 
                    Finalizado = false, 
                    Jornada = 3, 
                    CompeticionId = 1, 
                    EquipoLocalId = 2, 
                    EquipoVisitanteId = 4 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 5, 10, 19, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = null, 
                    GolesVisitantes = null, 
                    Finalizado = false, 
                    Jornada = 3, 
                    CompeticionId = 1, 
                    EquipoLocalId = 5, 
                    EquipoVisitanteId = 6 
                },

                // Liga Nacional (Competición 2)
                new Partido 
                { 
                    Fecha = new DateTime(2026, 4, 20, 19, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = 3, 
                    GolesVisitantes = 1, 
                    Finalizado = true, 
                    Jornada = 1, 
                    CompeticionId = 2, 
                    EquipoLocalId = 1, 
                    EquipoVisitanteId = 2 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 4, 20, 20, 30, 0, DateTimeKind.Utc), 
                    GolesLocal = 2, 
                    GolesVisitantes = 2, 
                    Finalizado = true, 
                    Jornada = 1, 
                    CompeticionId = 2, 
                    EquipoLocalId = 3, 
                    EquipoVisitanteId = 4 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 4, 27, 19, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = 1, 
                    GolesVisitantes = 0, 
                    Finalizado = true, 
                    Jornada = 2, 
                    CompeticionId = 2, 
                    EquipoLocalId = 5, 
                    EquipoVisitanteId = 6 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 4, 27, 20, 30, 0, DateTimeKind.Utc), 
                    GolesLocal = 2, 
                    GolesVisitantes = 1, 
                    Finalizado = true, 
                    Jornada = 2, 
                    CompeticionId = 2, 
                    EquipoLocalId = 2, 
                    EquipoVisitanteId = 1 
                },

                // Copa de Aficionados - Futbol 7 (Competición 3)
                new Partido 
                { 
                    Fecha = new DateTime(2026, 4, 22, 19, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = 4, 
                    GolesVisitantes = 3, 
                    Finalizado = true, 
                    Jornada = 1, 
                    CompeticionId = 3, 
                    EquipoLocalId = 1, 
                    EquipoVisitanteId = 2 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 4, 22, 20, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = 5, 
                    GolesVisitantes = 2, 
                    Finalizado = true, 
                    Jornada = 1, 
                    CompeticionId = 3, 
                    EquipoLocalId = 3, 
                    EquipoVisitanteId = 4 
                },
                new Partido 
                { 
                    Fecha = new DateTime(2026, 4, 29, 19, 0, 0, DateTimeKind.Utc), 
                    GolesLocal = 3, 
                    GolesVisitantes = 3, 
                    Finalizado = true, 
                    Jornada = 2, 
                    CompeticionId = 3, 
                    EquipoLocalId = 5, 
                    EquipoVisitanteId = 6 
                }
            };

            await context.Partidos.AddRangeAsync(partidos);
            await context.SaveChangesAsync();
        }
    }
}

