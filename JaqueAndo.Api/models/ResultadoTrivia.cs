namespace JaqueAndo.Api.Models;

public class ResultadoTrivia
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public int Puntaje { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
