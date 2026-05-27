using Derby.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Derby.Backend.Data;

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
                new Usuario { Email = "admin@derby.com", Contraseña = HashPassword("Admin@123"), Rol = Rol.Administrador },
                new Usuario { Email = "arbitro1@derby.com", Contraseña = HashPassword("Arbitro@123"), Rol = Rol.Arbitro },
                new Usuario { Email = "arbitro2@derby.com", Contraseña = HashPassword("Arbitro@123"), Rol = Rol.Arbitro },
                new Usuario { Email = "arbitro3@derby.com", Contraseña = HashPassword("Arbitro@123"), Rol = Rol.Arbitro },
                new Usuario { Email = "arbitro4@derby.com", Contraseña = HashPassword("Arbitro@123"), Rol = Rol.Arbitro },
            };
            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Usuarios creados");
        }
        else { Console.WriteLine("✓ Usuarios ya existen"); }

        // ─── Árbitros ─────────────────────────────────────────────────────────────
        var arbitrosExisten = await context.Arbitros.AnyAsync();
        if (!arbitrosExisten)
        {
            var arbitros = new List<Arbitro>
            {
                new Arbitro { Nombre = "Carlos",   Apellidos = "Martínez López",   NumeroColegiado = "ARB-001" },
                new Arbitro { Nombre = "Sergio",   Apellidos = "Ruiz Fernández",   NumeroColegiado = "ARB-002" },
                new Arbitro { Nombre = "Miguel",   Apellidos = "García Torres",    NumeroColegiado = "ARB-003" },
                new Arbitro { Nombre = "Alejandro",Apellidos = "Vidal Sánchez",    NumeroColegiado = "ARB-004" },
            };
            await context.Arbitros.AddRangeAsync(arbitros);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Árbitros creados");
        }
        else { Console.WriteLine("✓ Árbitros ya existen"); }

        // ─── Vincular usuarios árbitro ────────────────────────────────────────────
        var arbitros2 = await context.Arbitros.ToListAsync();
        var usuariosArbitro = await context.Usuarios.Where(u => u.Rol == Rol.Arbitro).ToListAsync();
        for (int i = 0; i < usuariosArbitro.Count && i < arbitros2.Count; i++)
        {
            if (usuariosArbitro[i].ArbitroId == null)
            {
                usuariosArbitro[i].ArbitroId = arbitros2[i].Id;
            }
        }
        await context.SaveChangesAsync();

        // ─── Equipos ─────────────────────────────────────────────────────────────
        var equiposExisten = await context.Equipos.AnyAsync();
        if (!equiposExisten)
        {
            var equipos = new List<Equipo>
            {
                // Primera DAW (8 equipos) — azules
                new Equipo { Nombre = "FC Derby Norte",       Sede = "Estadio El Pinar",        Entrenador = "Luis Enrique",        EscudoUrl = "https://ui-avatars.com/api/?name=FC+Derby+Norte&background=1565C0&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "Atlético Sur CF",      Sede = "Campo La Ribera",          Entrenador = "Diego Simeone",       EscudoUrl = "https://ui-avatars.com/api/?name=Atletico+Sur&background=0D47A1&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "CD Las Torres",        Sede = "Polideportivo Torres",     Entrenador = "Pep Guardiola",       EscudoUrl = "https://ui-avatars.com/api/?name=CD+Las+Torres&background=1976D2&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "Real Vallés FC",       Sede = "Estadio Vallés",           Entrenador = "Carlo Ancelotti",     EscudoUrl = "https://ui-avatars.com/api/?name=Real+Valles&background=2196F3&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "UD Miralba",           Sede = "Campo Miralba",            Entrenador = "Zinedine Zidane",     EscudoUrl = "https://ui-avatars.com/api/?name=UD+Miralba&background=0288D1&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "CF Esperanza",         Sede = "Polideportivo Esperanza",  Entrenador = "Jürgen Klopp",        EscudoUrl = "https://ui-avatars.com/api/?name=CF+Esperanza&background=01579B&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "Racing Derby Club",    Sede = "Estadio La Colina",        Entrenador = "José Mourinho",       EscudoUrl = "https://ui-avatars.com/api/?name=Racing+Derby&background=039BE5&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "Deportivo Crestall",   Sede = "Campo Crestall",           Entrenador = "Arsène Wenger",       EscudoUrl = "https://ui-avatars.com/api/?name=Dep+Crestall&background=0277BD&color=fff&size=128&bold=true" },

                // Segunda DAW (8 equipos) — rojos/granate
                new Equipo { Nombre = "CD Tres Ríos",         Sede = "Estadio Tres Ríos",        Entrenador = "Rafael Benítez",      EscudoUrl = "https://ui-avatars.com/api/?name=CD+Tres+Rios&background=C62828&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "Sporting Montaña",     Sede = "Campo La Cumbre",          Entrenador = "Marcelo Gallardo",    EscudoUrl = "https://ui-avatars.com/api/?name=Sporting+Montana&background=B71C1C&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "FC Los Álamos",        Sede = "Polideportivo Álamos",     Entrenador = "Jorge Sampaoli",      EscudoUrl = "https://ui-avatars.com/api/?name=FC+Los+Alamos&background=D32F2F&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "UD Piedralba",         Sede = "Estadio Piedralba",        Entrenador = "Ernesto Valverde",    EscudoUrl = "https://ui-avatars.com/api/?name=UD+Piedralba&background=E53935&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "CF Riobello",          Sede = "Campo Riobello",           Entrenador = "Mauricio Pochettino", EscudoUrl = "https://ui-avatars.com/api/?name=CF+Riobello&background=AD1457&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "SD Campofrío",         Sede = "Estadio Campofrío",        Entrenador = "Roberto Martínez",    EscudoUrl = "https://ui-avatars.com/api/?name=SD+Campofrio&background=880E4F&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "AC Puente Verde",      Sede = "Campo Puente Verde",       Entrenador = "Thomas Tuchel",       EscudoUrl = "https://ui-avatars.com/api/?name=AC+Puente+Verde&background=6A1B9A&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "UD Valderrama",        Sede = "Estadio Valderrama",       Entrenador = "Unai Emery",          EscudoUrl = "https://ui-avatars.com/api/?name=UD+Valderrama&background=4A148C&color=fff&size=128&bold=true" },

                // Tercera DAW (8 equipos) — verdes/naranja
                new Equipo { Nombre = "FC Monteclaro",        Sede = "Estadio Monteclaro",       Entrenador = "Antonio Conte",       EscudoUrl = "https://ui-avatars.com/api/?name=FC+Monteclaro&background=1B5E20&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "CD Estrella Roja",     Sede = "Campo Estrella",           Entrenador = "Quique Setién",       EscudoUrl = "https://ui-avatars.com/api/?name=CD+Estrella+Roja&background=2E7D32&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "SD Ribera Alta",       Sede = "Polideportivo Ribera",     Entrenador = "Michel",              EscudoUrl = "https://ui-avatars.com/api/?name=SD+Ribera+Alta&background=33691E&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "CF Altavista",         Sede = "Estadio Altavista",        Entrenador = "Niko Kovač",          EscudoUrl = "https://ui-avatars.com/api/?name=CF+Altavista&background=558B2F&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "Racing Norteño",       Sede = "Campo Norte",              Entrenador = "Rudi García",         EscudoUrl = "https://ui-avatars.com/api/?name=Racing+Norteno&background=E65100&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "UD Bellaverde",        Sede = "Estadio Bellaverde",       Entrenador = "Frank Lampard",       EscudoUrl = "https://ui-avatars.com/api/?name=UD+Bellaverde&background=BF360C&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "SC Maravillas",        Sede = "Campo Maravillas",         Entrenador = "Domenico Tedesco",    EscudoUrl = "https://ui-avatars.com/api/?name=SC+Maravillas&background=F57F17&color=fff&size=128&bold=true" },
                new Equipo { Nombre = "FC Tierrasol",         Sede = "Estadio Tierrasol",        Entrenador = "Oliver Glasner",      EscudoUrl = "https://ui-avatars.com/api/?name=FC+Tierrasol&background=E65100&color=fff&size=128&bold=true" },
            };
            await context.Equipos.AddRangeAsync(equipos);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ 24 equipos creados");
        }
        else { Console.WriteLine("✓ Equipos ya existen"); }

        // ─── Jugadores ───────────────────────────────────────────────────────────
        var jugadoresExisten = await context.Jugadores.AnyAsync();
        if (!jugadoresExisten)
        {
            var equipos = await context.Equipos.ToListAsync();
            var nombresPool = new[]
            {
                "Alejandro Gómez", "David Silva", "Marcos Llorente", "Pablo Sarabia", "Isco Alarcón",
                "Dani Olmo", "Mikel Merino", "Fabián Ruiz", "Pedri González", "Gavi Páez",
                "Ansu Fati", "Ferran Torres", "Bryan Gil", "Rodrigo Moreno", "Álvaro Morata",
                "Raúl de Tomás", "Gerard Moreno", "Carlos Soler", "Jesús Navas", "Jordi Alba",
                "Koke Resurrección", "Thiago Alcántara", "Sergio Busquets", "Cesar Azpilicueta", "Aymeric Laporte"
            };
            var jugadores = new List<Jugador>();
            int idx = 0;
            foreach (var equipo in equipos)
            {
                for (int dorsal = 1; dorsal <= 18; dorsal++)
                {
                    jugadores.Add(new Jugador
                    {
                        Nombre = nombresPool[idx % nombresPool.Length],
                        Dorsal = dorsal,
                        EquipoId = equipo.Id
                    });
                    idx++;
                }
            }
            await context.Jugadores.AddRangeAsync(jugadores);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ {jugadores.Count} jugadores creados");
        }
        else { Console.WriteLine("✓ Jugadores ya existen"); }

        // ─── Competiciones ───────────────────────────────────────────────────────
        var competicionesExisten = await context.Competiciones.AnyAsync();
        if (!competicionesExisten)
        {
            var competiciones = new List<Competicion>
            {
                new Competicion { Nombre = "Liga Derby",    Temporada = "2025-2026", TipoJuego = "futbol11", Grupo = "Grupo A" },
                new Competicion { Nombre = "Copa Derby",    Temporada = "2025-2026", TipoJuego = "futbol11", Grupo = "Grupo A" },
                new Competicion { Nombre = "Torneo Verano", Temporada = "2025-2026", TipoJuego = "futbol7",  Grupo = "Grupo B" },
            };
            await context.Competiciones.AddRangeAsync(competiciones);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Competiciones creadas");
        }
        else { Console.WriteLine("✓ Competiciones ya existen"); }

        // ─── Ligas ───────────────────────────────────────────────────────────────
        var ligasExisten = await context.Ligas.AnyAsync();
        if (!ligasExisten)
        {
            var competiciones = await context.Competiciones.ToListAsync();
            if (competiciones.Count >= 1)
            {
                var ligas = new List<Liga>
                {
                    new Liga { Nombre = "Primera DAW", CompeticionId = competiciones[0].Id, Grupo = "Único", Jornadas = 14, JornadaActual = 0, Estado = "Activo" },
                    new Liga { Nombre = "Segunda DAW", CompeticionId = competiciones[0].Id, Grupo = "Único", Jornadas = 14, JornadaActual = 0, Estado = "Activo" },
                    new Liga { Nombre = "Tercera DAW", CompeticionId = competiciones[0].Id, Grupo = "Único", Jornadas = 14, JornadaActual = 0, Estado = "Activo" },
                };
                await context.Ligas.AddRangeAsync(ligas);
                await context.SaveChangesAsync();
                Console.WriteLine("✓ Ligas creadas");
            }
        }
        else { Console.WriteLine("✓ Ligas ya existen"); }

        // ─── LigaEquipos ─────────────────────────────────────────────────────────
        var ligaEquiposExisten = await context.LigaEquipos.AnyAsync();
        if (!ligaEquiposExisten)
        {
            var ligas = await context.Ligas.ToListAsync();
            var equipos = await context.Equipos.ToListAsync();

            if (ligas.Count >= 3 && equipos.Count >= 24)
            {
                var ligaEquipos = new List<LigaEquipo>();

                for (int i = 0; i < 8; i++)
                    ligaEquipos.Add(new LigaEquipo { LigaId = ligas[0].Id, EquipoId = equipos[i].Id });

                for (int i = 8; i < 16; i++)
                    ligaEquipos.Add(new LigaEquipo { LigaId = ligas[1].Id, EquipoId = equipos[i].Id });

                for (int i = 16; i < 24; i++)
                    ligaEquipos.Add(new LigaEquipo { LigaId = ligas[2].Id, EquipoId = equipos[i].Id });

                await context.LigaEquipos.AddRangeAsync(ligaEquipos);
                await context.SaveChangesAsync();
                Console.WriteLine("✓ Equipos asignados a ligas");
            }
        }
        else { Console.WriteLine("✓ LigaEquipos ya existen"); }

        // ─── Partidos finalizados (historial + clasificación) ─────────────────────
        var partidosExisten = await context.Partidos.AnyAsync();
        if (!partidosExisten)
        {
            var ligas    = await context.Ligas.ToListAsync();
            var equipos  = await context.Equipos.ToListAsync();
            var arbitros = await context.Arbitros.ToListAsync();

            if (ligas.Count >= 1 && equipos.Count >= 8 && arbitros.Count >= 1)
            {
                // Equipos de Primera DAW (índices 0-7)
                var e = equipos.Take(8).ToList();
                var ligaId   = ligas[0].Id;
                var arbitro1 = arbitros[0].Id; // Carlos Martínez → vinculado a arbitro1@derby.com
                var arbitro2 = arbitros[1].Id;
                var arbitro3 = arbitros[2].Id;
                var arbitro4 = arbitros[3].Id;

                var hoy = DateTime.UtcNow;

                // Jornada 1 — 4 partidos finalizados
                var partidos = new List<Partido>
                {
                    new() { LigaId = ligaId, Jornada = 1, FechaHora = hoy.AddDays(-21),
                            EquipoLocalId = e[0].Id, EquipoVisitanteId = e[1].Id,
                            GolesLocal = 2, GolesVisitante = 1, Estado = "Finalizado", ArbitroId = arbitro1 },
                    new() { LigaId = ligaId, Jornada = 1, FechaHora = hoy.AddDays(-21),
                            EquipoLocalId = e[2].Id, EquipoVisitanteId = e[3].Id,
                            GolesLocal = 1, GolesVisitante = 1, Estado = "Finalizado", ArbitroId = arbitro2 },
                    new() { LigaId = ligaId, Jornada = 1, FechaHora = hoy.AddDays(-21),
                            EquipoLocalId = e[4].Id, EquipoVisitanteId = e[5].Id,
                            GolesLocal = 3, GolesVisitante = 0, Estado = "Finalizado", ArbitroId = arbitro3 },
                    new() { LigaId = ligaId, Jornada = 1, FechaHora = hoy.AddDays(-21),
                            EquipoLocalId = e[6].Id, EquipoVisitanteId = e[7].Id,
                            GolesLocal = 0, GolesVisitante = 2, Estado = "Finalizado", ArbitroId = arbitro4 },

                    // Jornada 2 — 4 partidos finalizados
                    new() { LigaId = ligaId, Jornada = 2, FechaHora = hoy.AddDays(-14),
                            EquipoLocalId = e[1].Id, EquipoVisitanteId = e[2].Id,
                            GolesLocal = 1, GolesVisitante = 0, Estado = "Finalizado", ArbitroId = arbitro1 },
                    new() { LigaId = ligaId, Jornada = 2, FechaHora = hoy.AddDays(-14),
                            EquipoLocalId = e[3].Id, EquipoVisitanteId = e[4].Id,
                            GolesLocal = 2, GolesVisitante = 2, Estado = "Finalizado", ArbitroId = arbitro2 },
                    new() { LigaId = ligaId, Jornada = 2, FechaHora = hoy.AddDays(-14),
                            EquipoLocalId = e[5].Id, EquipoVisitanteId = e[6].Id,
                            GolesLocal = 0, GolesVisitante = 1, Estado = "Finalizado", ArbitroId = arbitro3 },
                    new() { LigaId = ligaId, Jornada = 2, FechaHora = hoy.AddDays(-14),
                            EquipoLocalId = e[7].Id, EquipoVisitanteId = e[0].Id,
                            GolesLocal = 1, GolesVisitante = 3, Estado = "Finalizado", ArbitroId = arbitro4 },

                    // Jornada 3 — 4 partidos finalizados
                    new() { LigaId = ligaId, Jornada = 3, FechaHora = hoy.AddDays(-7),
                            EquipoLocalId = e[0].Id, EquipoVisitanteId = e[2].Id,
                            GolesLocal = 2, GolesVisitante = 0, Estado = "Finalizado", ArbitroId = arbitro1 },
                    new() { LigaId = ligaId, Jornada = 3, FechaHora = hoy.AddDays(-7),
                            EquipoLocalId = e[1].Id, EquipoVisitanteId = e[3].Id,
                            GolesLocal = 1, GolesVisitante = 2, Estado = "Finalizado", ArbitroId = arbitro2 },
                    new() { LigaId = ligaId, Jornada = 3, FechaHora = hoy.AddDays(-7),
                            EquipoLocalId = e[4].Id, EquipoVisitanteId = e[6].Id,
                            GolesLocal = 3, GolesVisitante = 1, Estado = "Finalizado", ArbitroId = arbitro3 },
                    new() { LigaId = ligaId, Jornada = 3, FechaHora = hoy.AddDays(-7),
                            EquipoLocalId = e[5].Id, EquipoVisitanteId = e[7].Id,
                            GolesLocal = 0, GolesVisitante = 0, Estado = "Finalizado", ArbitroId = arbitro4 },

                    // Jornada 4 — partido pendiente para tests E2E
                    new() { LigaId = ligaId, Jornada = 4, FechaHora = hoy.AddDays(7),
                            EquipoLocalId = e[0].Id, EquipoVisitanteId = e[1].Id,
                            Estado = "Programado", ArbitroId = arbitro1 },
                };

                await context.Partidos.AddRangeAsync(partidos);
                await context.SaveChangesAsync();

                // Actualizar JornadaActual de la liga
                ligas[0].JornadaActual = 3;
                await context.SaveChangesAsync();

                Console.WriteLine($"✓ {partidos.Count} partidos finalizados creados");

                // ─── Eventos de los partidos (goles y tarjetas) ───────────────────
                var jugadoresE0 = await context.Jugadores.Where(j => j.EquipoId == e[0].Id).Take(5).ToListAsync();
                var jugadoresE1 = await context.Jugadores.Where(j => j.EquipoId == e[1].Id).Take(5).ToListAsync();
                var jugadoresE4 = await context.Jugadores.Where(j => j.EquipoId == e[4].Id).Take(5).ToListAsync();
                var jugadoresE7 = await context.Jugadores.Where(j => j.EquipoId == e[7].Id).Take(5).ToListAsync();

                var partidosGuardados = await context.Partidos.OrderBy(p => p.Id).ToListAsync();

                if (jugadoresE0.Count >= 3 && jugadoresE1.Count >= 2 && partidosGuardados.Count >= 9)
                {
                    var eventos = new List<EventoPartido>
                    {
                        // Partido 1 (J1): e[0] 2-1 e[1]  — árbitro1
                        new() { PartidoId = partidosGuardados[0].Id, Minuto = 12, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE0[0].Id },
                        new() { PartidoId = partidosGuardados[0].Id, Minuto = 34, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE0[1].Id },
                        new() { PartidoId = partidosGuardados[0].Id, Minuto = 67, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE1[0].Id },
                        new() { PartidoId = partidosGuardados[0].Id, Minuto = 55, TipoEvento = TipoEvento.TarjetaAmarilla, JugadorId = jugadoresE1[1].Id },

                        // Partido 5 (J2): e[1] 1-0 e[2]  — árbitro1
                        new() { PartidoId = partidosGuardados[4].Id, Minuto = 78, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE1[0].Id },
                        new() { PartidoId = partidosGuardados[4].Id, Minuto = 22, TipoEvento = TipoEvento.TarjetaAmarilla, JugadorId = jugadoresE1[1].Id },

                        // Partido 9 (J3): e[0] 2-0 e[2]  — árbitro1
                        new() { PartidoId = partidosGuardados[8].Id, Minuto = 8,  TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE0[0].Id },
                        new() { PartidoId = partidosGuardados[8].Id, Minuto = 61, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE0[2].Id },
                        new() { PartidoId = partidosGuardados[8].Id, Minuto = 43, TipoEvento = TipoEvento.TarjetaRoja,    JugadorId = jugadoresE0[3].Id },

                        // Partido 3 (J1): e[4] 3-0 e[5]
                        new() { PartidoId = partidosGuardados[2].Id, Minuto = 5,  TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE4[0].Id },
                        new() { PartidoId = partidosGuardados[2].Id, Minuto = 29, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE4[1].Id },
                        new() { PartidoId = partidosGuardados[2].Id, Minuto = 88, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE4[2].Id },

                        // Partido 8 (J2): e[7] 1-3 e[0]
                        new() { PartidoId = partidosGuardados[7].Id, Minuto = 15, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE7[0].Id },
                        new() { PartidoId = partidosGuardados[7].Id, Minuto = 33, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE0[0].Id },
                        new() { PartidoId = partidosGuardados[7].Id, Minuto = 50, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE0[1].Id },
                        new() { PartidoId = partidosGuardados[7].Id, Minuto = 72, TipoEvento = TipoEvento.Gol,            JugadorId = jugadoresE0[2].Id },
                    };

                    await context.EventosPartidos.AddRangeAsync(eventos);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"✓ {eventos.Count} eventos de partido creados");
                }
            }
        }
        else { Console.WriteLine("✓ Partidos ya existen"); }
    }
}
