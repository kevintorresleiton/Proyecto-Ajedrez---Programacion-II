namespace JaqueAndo.Models;

public class PreguntaTrivia
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public string OpcionA { get; set; } = string.Empty;
    public string OpcionB { get; set; } = string.Empty;
    public string OpcionC { get; set; } = string.Empty;
    public string OpcionD { get; set; } = string.Empty;
    public int OpcionCorrecta { get; set; } // 1, 2, 3 o 4
    public string Explicacion { get; set; } = string.Empty;
}