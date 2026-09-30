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
    public string clock { get; set; } = string.Empty;
}

public class ChessApiService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<List<LichessTorneoDto>> ObtenerTorneosOficialesAsync()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("JaqueAndoApp/1.0");
            var response = await _httpClient.GetFromJsonAsync<Dictionary<string, List<LichessTorneoDto>>>("https://lichess.org/api/tournament");

            if (response != null && response.ContainsKey("created"))
            {
                return response["created"];
            }
        }
        catch
        {
            // Retorno de fallback en caso de falta de conexión a internet
        }

        return new List<LichessTorneoDto>
        {
            new LichessTorneoDto { id = "1", fullName = "Arena Semanal Blitz Lichess", perfType = "Blitz", minutes = 60 },
            new LichessTorneoDto { id = "2", fullName = "Torneo Abierto Rápido FIDE-FADA", perfType = "Rapid", minutes = 90 },
            new LichessTorneoDto { id = "3", fullName = "Maratón Internacional Clásico", perfType = "Classical", minutes = 120 }
        };
    }
}