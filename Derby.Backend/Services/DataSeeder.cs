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
                new Usuario
                {
                    Email = "admin@derby.com", Contraseña = HashPassword("Admin@123"), Rol = Rol.Administrador
                },
                new Usuario
                {
                    Email = "arbitro@derby.com", Contraseña = HashPassword("Arbitro@123"), Rol = Rol.Arbitro
                },
                new Usuario
                {
                    Email = "arbitro2@derby.com", Contraseña = HashPassword("Arbitro@123"), Rol = Rol.Arbitro
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

        // ─── Árbitros ─────────────────────────────────────────────────────────────
        var arbitrosExisten = await context.Arbitros.AnyAsync();
        if (!arbitrosExisten)
        {
            var arbitros = new List<Arbitro>
            {
                new Arbitro { Nombre = "Carlos", Apellidos = "Martínez López", NumeroColegiado = "ARB-001" },
                new Arbitro { Nombre = "Sergio", Apellidos = "Ruiz Fernández", NumeroColegiado = "ARB-002" },
                new Arbitro { Nombre = "Miguel", Apellidos = "García Torres", NumeroColegiado = "ARB-003" }
            };
            await context.Arbitros.AddRangeAsync(arbitros);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Árbitros creados");
        }
        else
        {
            Console.WriteLine("✓ Árbitros ya existen");
        }

        // ─── Vincular usuarios árbitro con sus registros ─────────────────────────
        var arbitrosParaVincular = await context.Arbitros.ToListAsync();
        var usuariosArbitro = await context.Usuarios.Where(u => u.Rol == Rol.Arbitro).ToListAsync();
        if (arbitrosParaVincular.Count >= 1 && usuariosArbitro.Any() && usuariosArbitro[0].ArbitroId == null)
        {
            usuariosArbitro[0].ArbitroId = arbitrosParaVincular[0].Id;
            if (usuariosArbitro.Count >= 2 && arbitrosParaVincular.Count >= 2)
                usuariosArbitro[1].ArbitroId = arbitrosParaVincular[1].Id;
            await context.SaveChangesAsync();
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
                new Equipo { Nombre = "FC Derby Norte", Sede = "Estadio El Pinar" },
                new Equipo { Nombre = "Atlético Sur CF", Sede = "Campo La Ribera" },
                new Equipo { Nombre = "CD Las Torres", Sede = "Polideportivo Torres" },
                new Equipo { Nombre = "Real Vallés FC", Sede = "Estadio Vallés" },
                new Equipo { Nombre = "UD Miralba", Sede = "Campo Miralba" },
                new Equipo { Nombre = "CF Esperanza", Sede = "Polideportivo Esperanza" },
                new Equipo { Nombre = "Racing Derby Club", Sede = "Estadio La Colina" },
                new Equipo { Nombre = "Deportivo Crestall", Sede = "Campo Crestall" },
                new Equipo { Nombre = "CD Tres Ríos", Sede = "Estadio Tres Ríos" },
                new Equipo { Nombre = "Sporting Montaña", Sede = "Campo La Cumbre" },
                new Equipo { Nombre = "FC Los Álamos", Sede = "Polideportivo Álamos" },
                new Equipo { Nombre = "UD Piedralba", Sede = "Estadio Piedralba" }
            };
            await context.Equipos.AddRangeAsync(equipos);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Equipos Derby creados");
        }
        else
        {
            Console.WriteLine("✓ Equipos ya existen");
        }

        // ─── Jugadores ───────────────────────────────────────────────────────────
        var jugadoresExisten = await context.Jugadores.AnyAsync();
        if (!jugadoresExisten)
        {
            Console.WriteLine("• Creando jugadores...");
            var equipos = await context.Equipos.ToListAsync();

            var jugadores = new List<Jugador>();

            // Nombres para generar jugadores
            var nombresPool = new[]
            {
                "Alejandro Gómez", "David Silva", "Marcos Llorente", "Pablo Sarabia", "Isco Alarcón",
                "Dani Olmo", "Mikel Merino", "Fabián Ruiz", "Pedri González", "Gavi Páez",
                "Ansu Fati", "Ferran Torres", "Bryan Gil", "Rodrigo Moreno", "Álvaro Morata",
                "Raúl de Tomás", "Gerard Moreno", "Carlos Soler", "Jesús Navas", "Jordi Alba"
            };

            int nombreIdx = 0;
            foreach (var equipo in equipos)
            {
                for (int dorsal = 1; dorsal <= 11; dorsal++)
                {
                    var nombre = nombreIdx < nombresPool.Length
                        ? nombresPool[nombreIdx]
                        : $"Jugador {nombreIdx + 1}";
                    jugadores.Add(new Jugador
                    {
                        Nombre = nombre,
                        Dorsal = dorsal,
                        EquipoId = equipo.Id
                    });
                    nombreIdx = (nombreIdx + 1) % nombresPool.Length;
                }
            }

            await context.Jugadores.AddRangeAsync(jugadores);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ {jugadores.Count} jugadores creados");
        }
        else
        {
            Console.WriteLine("✓ Jugadores ya existen");
        }

        // ─── Competiciones ───────────────────────────────────────────────────────
        var competicionesExisten = await context.Competiciones.AnyAsync();
        if (!competicionesExisten)
        {
            Console.WriteLine("• Creando competiciones...");
            var competiciones = new List<Competicion>
            {
                new Competicion
                    { Nombre = "Liga Derby", Temporada = "2025-2026", TipoJuego = "futbol11", Grupo = "Grupo A" },
                new Competicion
                    { Nombre = "Copa Derby", Temporada = "2025-2026", TipoJuego = "futbol11", Grupo = "Grupo A" },
                new Competicion
                    { Nombre = "Torneo Verano", Temporada = "2025-2026", TipoJuego = "futbol7", Grupo = "Grupo B" }
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
                    new Liga
                    {
                        Nombre = "Primera DAW", CompeticionId = competiciones[0].Id, Grupo = "Único", Jornadas = 22,
                        JornadaActual = 12, Estado = "Activo"
                    },
                    new Liga
                    {
                        Nombre = "Segunda DAW", CompeticionId = competiciones[0].Id, Grupo = "Único", Jornadas = 22,
                        JornadaActual = 18, Estado = "Activo"
                    },
                    new Liga
                    {
                        Nombre = "Tercera DAW", CompeticionId = competiciones[0].Id, Grupo = "Único", Jornadas = 22,
                        JornadaActual = 5, Estado = "Activo"
                    }
                };
                await context.Ligas.AddRangeAsync(ligas);
                await context.SaveChangesAsync();
                Console.WriteLine("✓ Ligas creadas");
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
            var arbitros = await context.Arbitros.ToListAsync();

            if (ligas.Count >= 3 && equipos.Count >= 12 && arbitros.Count >= 1)
            {
                var l1 = ligas[0];
                var l2 = ligas[1];
                var l3 = ligas[2];
                var arb = arbitros[0];

                var partidos = new List<Partido>
                {
                    // ── Primera DAW ──────────────────────────────────────────────
                    new Partido
                    {
                        Jornada = 1, LigaId = l1.Id, EquipoLocalId = equipos[0].Id, EquipoVisitanteId = equipos[1].Id,
                        GolesLocal = 2, GolesVisitante = 1, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 10, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 1, LigaId = l1.Id, EquipoLocalId = equipos[2].Id, EquipoVisitanteId = equipos[3].Id,
                        GolesLocal = 0, GolesVisitante = 0, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 10, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 2, LigaId = l1.Id, EquipoLocalId = equipos[1].Id, EquipoVisitanteId = equipos[2].Id,
                        GolesLocal = 3, GolesVisitante = 1, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 17, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 2, LigaId = l1.Id, EquipoLocalId = equipos[3].Id, EquipoVisitanteId = equipos[0].Id,
                        GolesLocal = 1, GolesVisitante = 2, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 17, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 3, LigaId = l1.Id, EquipoLocalId = equipos[0].Id, EquipoVisitanteId = equipos[2].Id,
                        GolesLocal = 1, GolesVisitante = 1, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 24, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 3, LigaId = l1.Id, EquipoLocalId = equipos[1].Id, EquipoVisitanteId = equipos[3].Id,
                        GolesLocal = 2, GolesVisitante = 0, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 24, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 13, LigaId = l1.Id, EquipoLocalId = equipos[2].Id, EquipoVisitanteId = equipos[0].Id,
                        GolesLocal = null, GolesVisitante = null, Estado = "Pendiente",
                        FechaHora = new DateTime(2026, 5, 16, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 13, LigaId = l1.Id, EquipoLocalId = equipos[3].Id, EquipoVisitanteId = equipos[1].Id,
                        GolesLocal = null, GolesVisitante = null, Estado = "Pendiente",
                        FechaHora = new DateTime(2026, 5, 16, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },

                    // ── Segunda DAW ──────────────────────────────────────────────
                    new Partido
                    {
                        Jornada = 1, LigaId = l2.Id, EquipoLocalId = equipos[4].Id, EquipoVisitanteId = equipos[5].Id,
                        GolesLocal = 4, GolesVisitante = 2, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 11, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 1, LigaId = l2.Id, EquipoLocalId = equipos[6].Id, EquipoVisitanteId = equipos[7].Id,
                        GolesLocal = 1, GolesVisitante = 3, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 11, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 2, LigaId = l2.Id, EquipoLocalId = equipos[5].Id, EquipoVisitanteId = equipos[6].Id,
                        GolesLocal = 2, GolesVisitante = 2, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 18, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 2, LigaId = l2.Id, EquipoLocalId = equipos[7].Id, EquipoVisitanteId = equipos[4].Id,
                        GolesLocal = 0, GolesVisitante = 1, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 18, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 3, LigaId = l2.Id, EquipoLocalId = equipos[4].Id, EquipoVisitanteId = equipos[6].Id,
                        GolesLocal = 3, GolesVisitante = 0, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 25, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 3, LigaId = l2.Id, EquipoLocalId = equipos[5].Id, EquipoVisitanteId = equipos[7].Id,
                        GolesLocal = 1, GolesVisitante = 2, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 1, 25, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 19, LigaId = l2.Id, EquipoLocalId = equipos[6].Id, EquipoVisitanteId = equipos[4].Id,
                        GolesLocal = null, GolesVisitante = null, Estado = "Pendiente",
                        FechaHora = new DateTime(2026, 5, 17, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 19, LigaId = l2.Id, EquipoLocalId = equipos[7].Id, EquipoVisitanteId = equipos[5].Id,
                        GolesLocal = null, GolesVisitante = null, Estado = "Pendiente",
                        FechaHora = new DateTime(2026, 5, 17, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },

                    // ── Tercera DAW ──────────────────────────────────────────────
                    new Partido
                    {
                        Jornada = 1, LigaId = l3.Id, EquipoLocalId = equipos[8].Id, EquipoVisitanteId = equipos[9].Id,
                        GolesLocal = 1, GolesVisitante = 0, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 3, 14, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 1, LigaId = l3.Id, EquipoLocalId = equipos[10].Id, EquipoVisitanteId = equipos[11].Id,
                        GolesLocal = 2, GolesVisitante = 3, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 3, 14, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 2, LigaId = l3.Id, EquipoLocalId = equipos[9].Id, EquipoVisitanteId = equipos[10].Id,
                        GolesLocal = 0, GolesVisitante = 1, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 3, 21, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 2, LigaId = l3.Id, EquipoLocalId = equipos[11].Id, EquipoVisitanteId = equipos[8].Id,
                        GolesLocal = 2, GolesVisitante = 2, Estado = "Finalizado",
                        FechaHora = new DateTime(2026, 3, 21, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 6, LigaId = l3.Id, EquipoLocalId = equipos[8].Id, EquipoVisitanteId = equipos[10].Id,
                        GolesLocal = null, GolesVisitante = null, Estado = "Pendiente",
                        FechaHora = new DateTime(2026, 5, 18, 18, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
                    new Partido
                    {
                        Jornada = 6, LigaId = l3.Id, EquipoLocalId = equipos[9].Id, EquipoVisitanteId = equipos[11].Id,
                        GolesLocal = null, GolesVisitante = null, Estado = "Pendiente",
                        FechaHora = new DateTime(2026, 5, 18, 20, 0, 0, DateTimeKind.Utc), ArbitroId = arb.Id
                    },
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

        // ─── Eventos de partido (para probar goleadores) ─────────────────────────
        var eventosExisten = await context.EventosPartidos.AnyAsync();
        if (!eventosExisten)
        {
            Console.WriteLine("• Creando eventos de partido...");

            var partidos = await context.Partidos.Where(p => p.Estado == "Finalizado").ToListAsync();
            var jugadores = await context.Jugadores.Include(j => j.Equipo).ToListAsync();

            if (partidos.Any() && jugadores.Any())
            {
                var eventos = new List<EventoPartido>();

                foreach (var partido in partidos.Take(6))
                {
                    var jugadoresLocal = jugadores.Where(j => j.EquipoId == partido.EquipoLocalId).ToList();
                    var jugadoresVisitante = jugadores.Where(j => j.EquipoId == partido.EquipoVisitanteId).ToList();

                    // Goles del equipo local
                    int golesLocal = partido.GolesLocal ?? 0;
                    for (int i = 0; i < golesLocal && i < jugadoresLocal.Count; i++)
                    {
                        eventos.Add(new EventoPartido
                        {
                            PartidoId = partido.Id,
                            JugadorId = jugadoresLocal[i % jugadoresLocal.Count].Id,
                            Minuto = 10 + (i * 15),
                            TipoEvento = TipoEvento.Gol
                        });
                    }

                    // Goles del equipo visitante
                    int golesVisitante = partido.GolesVisitante ?? 0;
                    for (int i = 0; i < golesVisitante && i < jugadoresVisitante.Count; i++)
                    {
                        eventos.Add(new EventoPartido
                        {
                            PartidoId = partido.Id,
                            JugadorId = jugadoresVisitante[i % jugadoresVisitante.Count].Id,
                            Minuto = 20 + (i * 20),
                            TipoEvento = TipoEvento.Gol
                        });
                    }

                    // Una tarjeta amarilla por partido
                    if (jugadoresLocal.Count > 2)
                    {
                        eventos.Add(new EventoPartido
                        {
                            PartidoId = partido.Id,
                            JugadorId = jugadoresLocal[2].Id,
                            Minuto = 55,
                            TipoEvento = TipoEvento.TarjetaAmarilla
                        });
                    }
                }

                await context.EventosPartidos.AddRangeAsync(eventos);
                await context.SaveChangesAsync();
                Console.WriteLine($"✓ {eventos.Count} eventos de partido creados");
            }
        }
        else
        {
            Console.WriteLine("✓ Eventos ya existen");
        }
    }
}