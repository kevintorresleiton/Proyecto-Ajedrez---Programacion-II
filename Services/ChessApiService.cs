using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace JaqueAndo.Services;

public class LichessTorneoDto
{
    public string id { get; set; } = string.Empty;
    public string fullName { get; set; } = string.Empty;
    public string perfType { get; set; } = string.Empty;
    public int minutes { get; set; }
}

public class LichessUserDto
{
    public string id { get; set; } = string.Empty;
    public string username { get; set; } = string.Empty;
    public bool online { get; set; }
    public LichessPerfsDto? perfs { get; set; }
    public LichessProfileDto? profile { get; set; }
    public string NombrePais => ChessApiService.ObtenerNombrePais(profile?.country);
}

public class LichessPerfsDto
{
    public LichessRatingDto? blitz { get; set; }
    public LichessRatingDto? rapid { get; set; }
    public LichessRatingDto? classical { get; set; }
}

public class LichessRatingDto
{
    public int rating { get; set; }
}

public class LichessProfileDto
{
    public string? country { get; set; }
    public string? location { get; set; }
    public string? bio { get; set; }
}

public class LichessTopResponse
{
    public List<LichessUserDto>? blitz { get; set; }
}

public class ChessApiService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    static ChessApiService()
    {
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("JaqueAndoApp/1.0");
    }

    public static async Task<List<LichessTorneoDto>> ObtenerTorneosOficialesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Dictionary<string, List<LichessTorneoDto>>>("https://lichess.org/api/tournament");
            if (response != null && response.ContainsKey("created")) return response["created"];
        }
        catch { }

        return new List<LichessTorneoDto>
        {
            new LichessTorneoDto { id = "1", fullName = "Arena Semanal Blitz Lichess", perfType = "Blitz", minutes = 60 },
            new LichessTorneoDto { id = "2", fullName = "Torneo Abierto Rápido FIDE", perfType = "Rapid", minutes = 90 }
        };
    }

    public static async Task<List<LichessUserDto>> ObtenerTopMundialAsync()
    {
        try
        {
            var top = await _httpClient.GetFromJsonAsync<LichessTopResponse>("https://lichess.org/player/top/10/blitz");
            if (top?.blitz != null && top.blitz.Count > 0) return top.blitz;
        }
        catch { }

        return new List<LichessUserDto>
        {
            new LichessUserDto { id = "magnuscarlsen", username = "MagnusCarlsen", perfs = new LichessPerfsDto { blitz = new LichessRatingDto { rating = 2882 } } },
            new LichessUserDto { id = "hikaru", username = "Hikaru", perfs = new LichessPerfsDto { blitz = new LichessRatingDto { rating = 2875 } } },
            new LichessUserDto { id = "alireza2003", username = "Alireza2003", perfs = new LichessPerfsDto { blitz = new LichessRatingDto { rating = 2810 } } }
        };
    }

    public static async Task<LichessUserDto?> BuscarUsuarioAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;
        try
        {
            return await _httpClient.GetFromJsonAsync<LichessUserDto>($"https://lichess.org/api/user/{username.Trim().ToLower()}");
        }
        catch
        {
            return null;
        }
    }

    public static string ObtenerNombrePais(string? countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode)) return "Sin especificar en Lichess";

        return countryCode.ToUpper() switch
        {
            "AR" => "Argentina",
            "NO" => "Noruega",
            "US" => "Estados Unidos",
            "FR" => "Francia",
            "RU" => "Rusia",
            "IN" => "India",
            "ES" => "España",
            "BR" => "Brasil",
            "CL" => "Chile",
            "UY" => "Uruguay",
            "DE" => "Alemania",
            "IT" => "Italia",
            _ => $"País ({countryCode.ToUpper()})"
        };
    }
}