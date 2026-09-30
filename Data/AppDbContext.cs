using JaqueAndo.Models;
using Microsoft.EntityFrameworkCore;

namespace JaqueAndo.Data;

public class AppDbContext : DbContext
{
    public DbSet<Jugador> Jugadores { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<PreguntaTrivia> Preguntas { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Crea el archivo SQLite 'ajedrez.db' en la carpeta de ejecución
            optionsBuilder.UseSqlite("Data Source=ajedrez.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sembrado inicial de usuarios para el Login
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Username = "admin", Password = "123", NombreCompleto = "Administrador del Sistema", EsAdmin = true },
            new Usuario { Id = 2, Username = "jugador", Password = "123", NombreCompleto = "Jugador Invitado", EsAdmin = false }
        );
    }
}