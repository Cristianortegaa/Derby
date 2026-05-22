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
                // Primera DAW (8 equipos)
                new Equipo { Nombre = "FC Derby Norte",       Sede = "Estadio El Pinar",        Entrenador = "Luis Enrique" },
                new Equipo { Nombre = "Atlético Sur CF",      Sede = "Campo La Ribera",          Entrenador = "Diego Simeone" },
                new Equipo { Nombre = "CD Las Torres",        Sede = "Polideportivo Torres",     Entrenador = "Pep Guardiola" },
                new Equipo { Nombre = "Real Vallés FC",       Sede = "Estadio Vallés",           Entrenador = "Carlo Ancelotti" },
                new Equipo { Nombre = "UD Miralba",           Sede = "Campo Miralba",            Entrenador = "Zinedine Zidane" },
                new Equipo { Nombre = "CF Esperanza",         Sede = "Polideportivo Esperanza",  Entrenador = "Jürgen Klopp" },
                new Equipo { Nombre = "Racing Derby Club",    Sede = "Estadio La Colina",        Entrenador = "José Mourinho" },
                new Equipo { Nombre = "Deportivo Crestall",   Sede = "Campo Crestall",           Entrenador = "Arsène Wenger" },

                // Segunda DAW (8 equipos)
                new Equipo { Nombre = "CD Tres Ríos",         Sede = "Estadio Tres Ríos",        Entrenador = "Rafael Benítez" },
                new Equipo { Nombre = "Sporting Montaña",     Sede = "Campo La Cumbre",          Entrenador = "Marcelo Gallardo" },
                new Equipo { Nombre = "FC Los Álamos",        Sede = "Polideportivo Álamos",     Entrenador = "Jorge Sampaoli" },
                new Equipo { Nombre = "UD Piedralba",         Sede = "Estadio Piedralba",        Entrenador = "Ernesto Valverde" },
                new Equipo { Nombre = "CF Riobello",          Sede = "Campo Riobello",           Entrenador = "Mauricio Pochettino" },
                new Equipo { Nombre = "SD Campofrío",         Sede = "Estadio Campofrío",        Entrenador = "Roberto Martínez" },
                new Equipo { Nombre = "AC Puente Verde",      Sede = "Campo Puente Verde",       Entrenador = "Thomas Tuchel" },
                new Equipo { Nombre = "UD Valderrama",        Sede = "Estadio Valderrama",       Entrenador = "Unai Emery" },

                // Tercera DAW (8 equipos)
                new Equipo { Nombre = "FC Monteclaro",        Sede = "Estadio Monteclaro",       Entrenador = "Antonio Conte" },
                new Equipo { Nombre = "CD Estrella Roja",     Sede = "Campo Estrella",           Entrenador = "Quique Setién" },
                new Equipo { Nombre = "SD Ribera Alta",       Sede = "Polideportivo Ribera",     Entrenador = "Michel" },
                new Equipo { Nombre = "CF Altavista",         Sede = "Estadio Altavista",        Entrenador = "Niko Kovač" },
                new Equipo { Nombre = "Racing Norteño",       Sede = "Campo Norte",              Entrenador = "Rudi García" },
                new Equipo { Nombre = "UD Bellaverde",        Sede = "Estadio Bellaverde",       Entrenador = "Frank Lampard" },
                new Equipo { Nombre = "SC Maravillas",        Sede = "Campo Maravillas",         Entrenador = "Domenico Tedesco" },
                new Equipo { Nombre = "FC Tierrasol",         Sede = "Estadio Tierrasol",        Entrenador = "Oliver Glasner" },
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
    }
}
