namespace JaqueAndo.Api.Models;

public class PreguntaTrivia
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public string OpcionA { get; set; } = string.Empty;
    public string OpcionB { get; set; } = string.Empty;
    public string OpcionC { get; set; } = string.Empty;
    public string OpcionD { get; set; } = string.Empty;
    public int OpcionCorrecta { get; set; }
    public string Explicacion { get; set; } = string.Empty;
}
