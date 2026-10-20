using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JaqueAndo.Models;

namespace JaqueAndo.Services;

public class ClubDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;

    // El ComboBox muestra el texto que devuelve ToString()
    public override string ToString() => Nombre;
}

public class JugadorDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string IdFide { get; set; } = "N/A";
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = "M";
    public bool EsSocio { get; set; }
    public int EloEstimado { get; set; } = 1200;
    public int? ClubId { get; set; }
    public ClubDto? Club { get; set; }

    [JsonIgnore]
    public string ClubNombre => Club?.Nombre ?? "Libre";
}

public static class ApiService
{
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("http://localhost:5207/"),
        Timeout = TimeSpan.FromSeconds(10)
    };

    public record LoginRequest(string Username, string Password);
    public record NuevoResultado(int UsuarioId, int Puntaje);

    private const string ErrorConexion = "No se pudo conectar con la API. ¿Está corriendo?";
    private const string ErrorTimeout = "La API tardó demasiado en responder.";

    private static string ErrorHttp(System.Net.HttpStatusCode code) => code switch
    {
        System.Net.HttpStatusCode.NotFound => "El registro ya no existe.",
        System.Net.HttpStatusCode.BadRequest => "Los datos enviados no son válidos.",
        _ => $"Error del servidor ({(int)code})."
    };

    // Ejecuta una llamada que no devuelve datos. Devuelve null si salió bien, o el mensaje de error.
    private static async Task<string?> EnviarAsync(Func<Task<HttpResponseMessage>> accion)
    {
        try
        {
            var resp = await accion();
            return resp.IsSuccessStatusCode ? null : ErrorHttp(resp.StatusCode);
        }
        catch (HttpRequestException) { return ErrorConexion; }
        catch (TaskCanceledException) { return ErrorTimeout; }
    }

    // ---------- Login ----------
    public static async Task<(Usuario? Usuario, string? Error)> LoginAsync(string username, string password)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/usuarios/login", new LoginRequest(username, password));

            if (resp.IsSuccessStatusCode)
                return (await resp.Content.ReadFromJsonAsync<Usuario>(), null);

            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
                return (null, "Usuario o contraseña incorrectos.");

            return (null, $"Error del servidor ({(int)resp.StatusCode}).");
        }
        catch (HttpRequestException) { return (null, ErrorConexion); }
        catch (TaskCanceledException) { return (null, ErrorTimeout); }
    }

    // ---------- Trivia ----------
    public static async Task<(List<PreguntaTrivia>? Preguntas, string? Error)> ObtenerPreguntasAsync(int cantidad = 5)
    {
        try
        {
            var resp = await _http.GetAsync($"api/preguntas?cantidad={cantidad}");
            if (!resp.IsSuccessStatusCode)
                return (null, $"Error del servidor ({(int)resp.StatusCode}).");

            return (await resp.Content.ReadFromJsonAsync<List<PreguntaTrivia>>(), null);
        }
        catch (HttpRequestException) { return (null, ErrorConexion); }
        catch (TaskCanceledException) { return (null, ErrorTimeout); }
    }

    public static async Task<string?> GuardarResultadoTriviaAsync(int usuarioId, int puntaje)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/resultadostrivia", new NuevoResultado(usuarioId, puntaje));
            return resp.IsSuccessStatusCode ? null : $"No se pudo guardar el puntaje ({(int)resp.StatusCode}).";
        }
        catch (HttpRequestException) { return ErrorConexion; }
        catch (TaskCanceledException) { return ErrorTimeout; }
    }

    // ---------- Jugadores y clubes ----------
    public static async Task<(List<JugadorDto>? Jugadores, string? Error)> ObtenerJugadoresAsync()
    {
        try
        {
            var resp = await _http.GetAsync("api/jugadores");
            if (!resp.IsSuccessStatusCode)
                return (null, ErrorHttp(resp.StatusCode));

            return (await resp.Content.ReadFromJsonAsync<List<JugadorDto>>(), null);
        }
        catch (HttpRequestException) { return (null, ErrorConexion); }
        catch (TaskCanceledException) { return (null, ErrorTimeout); }
    }

    public static async Task<(List<ClubDto>? Clubes, string? Error)> ObtenerClubesAsync()
    {
        try
        {
            var resp = await _http.GetAsync("api/clubes");
            if (!resp.IsSuccessStatusCode)
                return (null, ErrorHttp(resp.StatusCode));

            return (await resp.Content.ReadFromJsonAsync<List<ClubDto>>(), null);
        }
        catch (HttpRequestException) { return (null, ErrorConexion); }
        catch (TaskCanceledException) { return (null, ErrorTimeout); }
    }

    public static Task<string?> CrearJugadorAsync(JugadorDto j) =>
        EnviarAsync(() => _http.PostAsJsonAsync("api/jugadores", j));

    public static Task<string?> ActualizarJugadorAsync(JugadorDto j) =>
        EnviarAsync(() => _http.PutAsJsonAsync($"api/jugadores/{j.Id}", j));

    public static Task<string?> EliminarJugadorAsync(int id) =>
        EnviarAsync(() => _http.DeleteAsync($"api/jugadores/{id}"));
}