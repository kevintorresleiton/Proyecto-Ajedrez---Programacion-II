using System;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;
using JaqueAndo.Data;
using JaqueAndo.Models;

namespace JaqueAndo.Services;

public class ExcelImporterService
{
    public static void SeedJugadoresDesdeExcel()
    {
        using var context = new AppDbContext();
        context.Database.EnsureCreated();

        // Si ya existen registros, no volvemos a importar
        if (context.Jugadores.Any()) return;

        string excelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Jugadores_Ajedrez_BaseDeDatos.xlsx");

        // Si no lo encuentra en la carpeta de salida, intenta en la carpeta local del proyecto
        if (!File.Exists(excelPath))
        {
            excelPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Jugadores_Ajedrez_BaseDeDatos.xlsx");
        }

        if (!File.Exists(excelPath))
        {
            Console.WriteLine($">>> Archivo Excel no encontrado en: {excelPath}");
            return;
        }

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        int fila = 0;
        while (reader.Read())
        {
            fila++;
            if (fila == 1) continue; // Saltar encabezados

            string nombre = reader.GetValue(1)?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(nombre)) continue;

            string apellido = reader.GetValue(2)?.ToString() ?? "";
            string idFide = reader.GetValue(3)?.ToString() ?? "N/A";
            
            // Procesar Fecha de Nacimiento
            DateTime fechaNac = DateTime.Now.AddYears(-20);
            var fechaVal = reader.GetValue(4);
            if (fechaVal is DateTime dt)
            {
                fechaNac = dt;
            }
            else if (fechaVal != null)
            {
                DateTime.TryParse(fechaVal.ToString(), out fechaNac);
            }

            string sexo = reader.GetValue(5)?.ToString() ?? "M";
            string esSocioVal = reader.GetValue(6)?.ToString()?.Trim().ToLower() ?? "";
            bool esSocio = esSocioVal == "sí" || esSocioVal == "si" || esSocioVal == "true" || esSocioVal == "1";

            var jugador = new Jugador
            {
                Nombre = nombre,
                Apellido = apellido,
                IdFide = idFide,
                FechaNacimiento = fechaNac,
                Sexo = sexo,
                EsSocio = esSocio,
                Club = "Libre / Ninguno",
                EloEstimado = 1200
            };

            context.Jugadores.Add(jugador);
        }

        context.SaveChanges();
        Console.WriteLine($">>> ¡Se importaron correctamente {context.Jugadores.Count()} jugadores a SQLite!");
    }
}