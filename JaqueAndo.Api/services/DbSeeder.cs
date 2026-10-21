using System.Text;
using ExcelDataReader;
using JaqueAndo.Api.Data;
using JaqueAndo.Api.Models;

namespace JaqueAndo.Api.Services;

public static class DbSeeder
{
    public static void Seed(AppDbContext db, string contentRoot)
    {
        SeedClubes(db);
        SeedPreguntas(db);
        SeedJugadoresDesdeExcel(db, contentRoot);
    }

    private static void SeedClubes(AppDbContext db)
    {
        if (db.Clubes.Any()) return;
        db.Clubes.AddRange(
            new Club { Nombre = "Club Argentino de Ajedrez", Ciudad = "Buenos Aires" },
            new Club { Nombre = "Club Jaque Mate", Ciudad = "Corrientes" },
            new Club { Nombre = "Club Torre Blanca", Ciudad = "Rosario" });
        db.SaveChanges();
    }

    private static void SeedPreguntas(AppDbContext db)
    {
        if (db.Preguntas.Any()) return;
        db.Preguntas.AddRange(
            new PreguntaTrivia { Enunciado = "¿Cuál es la única pieza que puede saltar sobre otras?", OpcionA = "Torre", OpcionB = "Caballo", OpcionC = "Alfil", OpcionD = "Dama", OpcionCorrecta = 2, Explicacion = "El Caballo puede saltar sobre otras piezas." },
            new PreguntaTrivia { Enunciado = "¿Cuántas casillas tiene un tablero de ajedrez estándar?", OpcionA = "36", OpcionB = "48", OpcionC = "64", OpcionD = "81", OpcionCorrecta = 3, Explicacion = "El tablero tiene 8 filas por 8 columnas: 64 casillas." },
            new PreguntaTrivia { Enunciado = "¿Qué movimiento permite mover el Rey y la Torre?", OpcionA = "Enroque", OpcionB = "Peón al paso", OpcionC = "Promoción", OpcionD = "Coronación", OpcionCorrecta = 1, Explicacion = "El enroque desplaza al Rey y a una Torre." },
            new PreguntaTrivia { Enunciado = "¿Quién fue el primer Campeón Mundial oficial de ajedrez?", OpcionA = "Garry Kasparov", OpcionB = "Wilhelm Steinitz", OpcionC = "Bobby Fischer", OpcionD = "José Raúl Capablanca", OpcionCorrecta = 2, Explicacion = "Wilhelm Steinitz fue campeón mundial en 1886." },
            new PreguntaTrivia { Enunciado = "¿Qué valor aproximado tiene la Dama?", OpcionA = "3 puntos", OpcionB = "5 puntos", OpcionC = "9 puntos", OpcionD = "10 puntos", OpcionCorrecta = 3, Explicacion = "La Dama suele valorarse en 9 puntos." });
        db.SaveChanges();
    }

    private static void SeedJugadoresDesdeExcel(AppDbContext db, string contentRoot)
    {
        if (db.Jugadores.Any()) return;

        var path = Path.Combine(contentRoot, "data", "Jugadores_Ajedrez_BaseDeDatos.xlsx");
        if (!File.Exists(path))
        {
            Console.WriteLine($">>> Excel no encontrado en: {path}");
            return;
        }

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        int fila = 0;
        while (reader.Read())
        {
            fila++;
            if (fila == 1) continue; // encabezados

            var nombre = reader.GetValue(1)?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(nombre)) continue;

            var fechaNac = DateTime.Now.AddYears(-20);
            var fechaVal = reader.GetValue(4);
            if (fechaVal is DateTime dt) fechaNac = dt;
            else if (fechaVal != null && DateTime.TryParse(fechaVal.ToString(), out var parsed)) fechaNac = parsed;

            var socio = reader.GetValue(6)?.ToString()?.Trim().ToLower() ?? "";

            db.Jugadores.Add(new Jugador
            {
                Nombre = nombre,
                Apellido = reader.GetValue(2)?.ToString() ?? "",
                IdFide = reader.GetValue(3)?.ToString() ?? "N/A",
                FechaNacimiento = fechaNac,
                Sexo = reader.GetValue(5)?.ToString() ?? "M",
                EsSocio = socio is "sí" or "si" or "true" or "1",
                EloEstimado = 1200
            });
        }

        db.SaveChanges();
        Console.WriteLine($">>> Se importaron {db.Jugadores.Count()} jugadores.");
    }
}