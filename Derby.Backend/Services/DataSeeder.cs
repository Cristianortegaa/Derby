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
        // ─── Usuarios ────────────────────────────────────────────────────────────
        var usuariosExisten = await context.Usuarios.AnyAsync();
        if (!usuariosExisten)
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Email = "admin@derby.com",   Contraseña = HashPassword("Admin@123"),   Rol = Rol.Administrador },
                new Usuario { Email = "arbitro@derby.com", Contraseña = HashPassword("Arbitro@123"), Rol = Rol.Arbitro }
            };
            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Usuarios creados");
        }
        else
        {
            Console.WriteLine("✓ Usuarios ya existen");
        }

        // ─── Equipos ─────────────────────────────────────────────────────────────
        var equiposActuales = await context.Equipos.ToListAsync();
        var necesitaRecrearEquipos = equiposActuales.Count == 0 || equiposActuales.Any(e => e.Nombre == "Getafe CF");
        if (necesitaRecrearEquipos)
        {
            Console.WriteLine("• Creando equipos Derby...");
            if (equiposActuales.Count > 0)
            {
                context.Equipos.RemoveRange(equiposActuales);
                await context.SaveChangesAsync();
            }
            var equipos = new List<Equipo>
            {
                new Equipo { Nombre = "FC Derby Norte",     Sede = "Estadio El Pinar"        },
                new Equipo { Nombre = "Atlético Sur CF",    Sede = "Campo La Ribera"           },
                new Equipo { Nombre = "CD Las Torres",      Sede = "Polideportivo Torres"      },
                new Equipo { Nombre = "Real Vallés FC",     Sede = "Estadio Vallés"            },
                new Equipo { Nombre = "UD Miralba",         Sede = "Campo Miralba"             },
                new Equipo { Nombre = "CF Esperanza",       Sede = "Polideportivo Esperanza"   },
                new Equipo { Nombre = "Racing Derby Club",  Sede = "Estadio La Colina"         },
                new Equipo { Nombre = "Deportivo Crestall", Sede = "Campo Crestall"            },
                new Equipo { Nombre = "CD Tres Ríos",       Sede = "Estadio Tres Ríos"         },
                new Equipo { Nombre = "Sporting Montaña",   Sede = "Campo La Cumbre"           },
                new Equipo { Nombre = "FC Los Álamos",      Sede = "Polideportivo Álamos"      },
                new Equipo { Nombre = "UD Piedralba",       Sede = "Estadio Piedralba"         }
            };
            await context.Equipos.AddRangeAsync(equipos);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Equipos Derby creados");
        }
        else
        {
            Console.WriteLine("✓ Equipos ya existen");
        }

        // ─── Competiciones ───────────────────────────────────────────────────────
        var competicionesExisten = await context.Competiciones.AnyAsync();
        if (!competicionesExisten)
        {
            Console.WriteLine("• Creando competiciones...");
            var competiciones = new List<Competicion>
            {
                new Competicion { Nombre = "Liga Derby",         Temporada = "2025-2026", TipoJuego = "futbol11", Grupo = "Grupo A" },
                new Competicion { Nombre = "Copa Derby",         Temporada = "2025-2026", TipoJuego = "futbol11", Grupo = "Grupo A" },
                new Competicion { Nombre = "Torneo Verano",      Temporada = "2025-2026", TipoJuego = "futbol7",  Grupo = "Grupo B" }
            };
            await context.Competiciones.AddRangeAsync(competiciones);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Competiciones creadas");
        }
        else
        {
            Console.WriteLine("✓ Competiciones ya existen");
        }

        // ─── Ligas ───────────────────────────────────────────────────────────────
        var ligasExisten = await context.Ligas.AnyAsync();
        if (!ligasExisten)
        {
            Console.WriteLine("• Creando ligas...");
            var competiciones = await context.Competiciones.ToListAsync();
            if (competiciones.Count >= 1)
            {
                var ligas = new List<Liga>
                {
                    new Liga { Nombre = "Primera DAW",  CompeticionId = competiciones[0].Id, Grupo = "Único", Jornadas = 22, JornadaActual = 12, Estado = "Activo" },
                    new Liga { Nombre = "Segunda DAW",  CompeticionId = competiciones[0].Id, Grupo = "Único", Jornadas = 22, JornadaActual = 18, Estado = "Activo" },
                    new Liga { Nombre = "Tercera DAW",  CompeticionId = competiciones[0].Id, Grupo = "Único", Jornadas = 22, JornadaActual = 5,  Estado = "Activo" }
                };
                await context.Ligas.AddRangeAsync(ligas);
                await context.SaveChangesAsync();
                Console.WriteLine(" Ligas creadas");
            }
        }
        else
        {
            Console.WriteLine("✓ Ligas ya existen");
        }

        // ─── Partidos ────────────────────────────────────────────────────────────
        var partidosExisten = await context.Partidos.AnyAsync();
        if (!partidosExisten)
        {
            Console.WriteLine("• Creando partidos...");
            var ligas = await context.Ligas.ToListAsync();
            var equipos = await context.Equipos.ToListAsync();

            if (ligas.Count >= 3 && equipos.Count >= 12)
            {
                var l1 = ligas[0]; // Primera DAW
                var l2 = ligas[1]; // Segunda DAW
                var l3 = ligas[2]; // Tercera DAW

                // Equipos por bloques de 4 para cada liga
                var partidos = new List<Partido>
                {
                    // ── Primera DAW ──────────────────────────────────────────────
                    // Jornada 1
                    new Partido { Jornada = 1,  LigaId = l1.Id, EquipoLocalId = equipos[0].Id,  EquipoVisitanteId = equipos[1].Id,  GolesLocal = 2, GolesVisitante = 1, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 10, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 1,  LigaId = l1.Id, EquipoLocalId = equipos[2].Id,  EquipoVisitanteId = equipos[3].Id,  GolesLocal = 0, GolesVisitante = 0, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    // Jornada 2
                    new Partido { Jornada = 2,  LigaId = l1.Id, EquipoLocalId = equipos[1].Id,  EquipoVisitanteId = equipos[2].Id,  GolesLocal = 3, GolesVisitante = 1, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 17, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 2,  LigaId = l1.Id, EquipoLocalId = equipos[3].Id,  EquipoVisitanteId = equipos[0].Id,  GolesLocal = 1, GolesVisitante = 2, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 17, 20, 0, 0, DateTimeKind.Utc) },
                    // Jornada 3
                    new Partido { Jornada = 3,  LigaId = l1.Id, EquipoLocalId = equipos[0].Id,  EquipoVisitanteId = equipos[2].Id,  GolesLocal = 1, GolesVisitante = 1, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 24, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 3,  LigaId = l1.Id, EquipoLocalId = equipos[1].Id,  EquipoVisitanteId = equipos[3].Id,  GolesLocal = 2, GolesVisitante = 0, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 24, 20, 0, 0, DateTimeKind.Utc) },
                    // Jornada 13 (próxima pendiente)
                    new Partido { Jornada = 13, LigaId = l1.Id, EquipoLocalId = equipos[2].Id,  EquipoVisitanteId = equipos[0].Id,  GolesLocal = null, GolesVisitante = null, Estado = "Pendiente", FechaHora = new DateTime(2026, 5, 16, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 13, LigaId = l1.Id, EquipoLocalId = equipos[3].Id,  EquipoVisitanteId = equipos[1].Id,  GolesLocal = null, GolesVisitante = null, Estado = "Pendiente", FechaHora = new DateTime(2026, 5, 16, 20, 0, 0, DateTimeKind.Utc) },

                    // ── Segunda DAW ──────────────────────────────────────────────
                    // Jornada 1
                    new Partido { Jornada = 1,  LigaId = l2.Id, EquipoLocalId = equipos[4].Id,  EquipoVisitanteId = equipos[5].Id,  GolesLocal = 4, GolesVisitante = 2, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 11, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 1,  LigaId = l2.Id, EquipoLocalId = equipos[6].Id,  EquipoVisitanteId = equipos[7].Id,  GolesLocal = 1, GolesVisitante = 3, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 11, 20, 0, 0, DateTimeKind.Utc) },
                    // Jornada 2
                    new Partido { Jornada = 2,  LigaId = l2.Id, EquipoLocalId = equipos[5].Id,  EquipoVisitanteId = equipos[6].Id,  GolesLocal = 2, GolesVisitante = 2, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 18, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 2,  LigaId = l2.Id, EquipoLocalId = equipos[7].Id,  EquipoVisitanteId = equipos[4].Id,  GolesLocal = 0, GolesVisitante = 1, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 18, 20, 0, 0, DateTimeKind.Utc) },
                    // Jornada 3
                    new Partido { Jornada = 3,  LigaId = l2.Id, EquipoLocalId = equipos[4].Id,  EquipoVisitanteId = equipos[6].Id,  GolesLocal = 3, GolesVisitante = 0, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 25, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 3,  LigaId = l2.Id, EquipoLocalId = equipos[5].Id,  EquipoVisitanteId = equipos[7].Id,  GolesLocal = 1, GolesVisitante = 2, Estado = "Finalizado", FechaHora = new DateTime(2026, 1, 25, 20, 0, 0, DateTimeKind.Utc) },
                    // Jornada 19 (próxima pendiente)
                    new Partido { Jornada = 19, LigaId = l2.Id, EquipoLocalId = equipos[6].Id,  EquipoVisitanteId = equipos[4].Id,  GolesLocal = null, GolesVisitante = null, Estado = "Pendiente", FechaHora = new DateTime(2026, 5, 17, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 19, LigaId = l2.Id, EquipoLocalId = equipos[7].Id,  EquipoVisitanteId = equipos[5].Id,  GolesLocal = null, GolesVisitante = null, Estado = "Pendiente", FechaHora = new DateTime(2026, 5, 17, 20, 0, 0, DateTimeKind.Utc) },

                    // ── Tercera DAW ──────────────────────────────────────────────
                    // Jornada 1
                    new Partido { Jornada = 1,  LigaId = l3.Id, EquipoLocalId = equipos[8].Id,  EquipoVisitanteId = equipos[9].Id,  GolesLocal = 1, GolesVisitante = 0, Estado = "Finalizado", FechaHora = new DateTime(2026, 3, 14, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 1,  LigaId = l3.Id, EquipoLocalId = equipos[10].Id, EquipoVisitanteId = equipos[11].Id, GolesLocal = 2, GolesVisitante = 3, Estado = "Finalizado", FechaHora = new DateTime(2026, 3, 14, 20, 0, 0, DateTimeKind.Utc) },
                    // Jornada 2
                    new Partido { Jornada = 2,  LigaId = l3.Id, EquipoLocalId = equipos[9].Id,  EquipoVisitanteId = equipos[10].Id, GolesLocal = 0, GolesVisitante = 1, Estado = "Finalizado", FechaHora = new DateTime(2026, 3, 21, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 2,  LigaId = l3.Id, EquipoLocalId = equipos[11].Id, EquipoVisitanteId = equipos[8].Id,  GolesLocal = 2, GolesVisitante = 2, Estado = "Finalizado", FechaHora = new DateTime(2026, 3, 21, 20, 0, 0, DateTimeKind.Utc) },
                    // Jornada 6 (próxima pendiente)
                    new Partido { Jornada = 6,  LigaId = l3.Id, EquipoLocalId = equipos[8].Id,  EquipoVisitanteId = equipos[10].Id, GolesLocal = null, GolesVisitante = null, Estado = "Pendiente", FechaHora = new DateTime(2026, 5, 18, 18, 0, 0, DateTimeKind.Utc) },
                    new Partido { Jornada = 6,  LigaId = l3.Id, EquipoLocalId = equipos[9].Id,  EquipoVisitanteId = equipos[11].Id, GolesLocal = null, GolesVisitante = null, Estado = "Pendiente", FechaHora = new DateTime(2026, 5, 18, 20, 0, 0, DateTimeKind.Utc) },
                };

                await context.Partidos.AddRangeAsync(partidos);
                await context.SaveChangesAsync();
                Console.WriteLine("✓ Partidos creados");
            }
        }
        else
        {
            Console.WriteLine("✓ Partidos ya existen");
        }
    }
}
