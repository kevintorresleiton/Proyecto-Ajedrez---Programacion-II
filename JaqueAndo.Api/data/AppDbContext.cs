using JaqueAndo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JaqueAndo.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Jugador> Jugadores => Set<Jugador>();
    public DbSet<PreguntaTrivia> Preguntas => Set<PreguntaTrivia>();
    public DbSet<Club> Clubes => Set<Club>();
    public DbSet<ResultadoTrivia> ResultadosTrivia => Set<ResultadoTrivia>();
    public DbSet<Partida> Partidas => Set<Partida>();
    public DbSet<Torneo> Torneos => Set<Torneo>();
    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Usuario>().HasIndex(u => u.Username).IsUnique();
        mb.Entity<Inscripcion>().HasKey(i => new { i.TorneoId, i.JugadorId });

        // Partida tiene dos FK a Jugador: sin esto EF da error de ciclos
        mb.Entity<Partida>().HasOne(p => p.JugadorBlancas).WithMany()
            .HasForeignKey(p => p.JugadorBlancasId).OnDelete(DeleteBehavior.Restrict);
        mb.Entity<Partida>().HasOne(p => p.JugadorNegras).WithMany()
            .HasForeignKey(p => p.JugadorNegrasId).OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Username = "admin", Password = "123", NombreCompleto = "Administrador del Sistema", EsAdmin = true },
            new Usuario { Id = 2, Username = "jugador", Password = "123", NombreCompleto = "Jugador Invitado", EsAdmin = false }
        );
    }
}