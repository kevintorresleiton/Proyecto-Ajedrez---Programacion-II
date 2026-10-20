using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using JaqueAndo.Models;

namespace JaqueAndo.Services;

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

    public static async Task<(List<PreguntaTrivia>? Preguntas, string? Error)> ObtenerPreguntasAsync(int cantidad = 5)
    {
        try
        {
            var resp = await _http.GetAsync($"api/preguntas?cantidad={cantidad}");
            if (!resp.IsSuccessStatusCode)
                return (null, $"Error del servidor ({(int)resp.StatusCode}).");

            var lista = await resp.Content.ReadFromJsonAsync<List<PreguntaTrivia>>();
            return (lista, null);
        }
        catch (HttpRequestException) { return (null, ErrorConexion); }
        catch (TaskCanceledException) { return (null, ErrorTimeout); }
    }

    // Devuelve null si se guardó bien, o el mensaje de error
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
}