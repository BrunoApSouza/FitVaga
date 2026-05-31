namespace FitVaga.API.Models;

public class Vaga
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
}