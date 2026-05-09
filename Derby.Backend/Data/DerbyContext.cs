using Derby.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Data;

public class DerbyContext: DbContext
{
    public DerbyContext(DbContextOptions<DerbyContext> options) : base(options) {}
    
    public DbSet<Equipo> Equipos { get; set; }
    public DbSet<Jugador> Jugadores { get; set; }
    public DbSet<Partido>  Partidos { get; set; }
    public DbSet<Entrenador> Entrenadores { get; set; }
    public DbSet<Arbitro> Arbitros { get; set; }
    public DbSet<EventoPartido> EventosPartidos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Competicion> Competiciones { get; set; }
    public DbSet<Liga> Ligas { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Partido>()
            .HasOne(p => p.EquipoLocal)
            .WithMany()
            .HasForeignKey(p => p.EquipoLocalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Partido>()
            .HasOne(p => p.EquipoVisitante)
            .WithMany()
            .HasForeignKey(p => p.EquipoVisitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Partido>()
            .HasOne(p => p.Liga)
            .WithMany()
            .HasForeignKey(p => p.LigaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Rol)
            .HasConversion<string>();
    }
}