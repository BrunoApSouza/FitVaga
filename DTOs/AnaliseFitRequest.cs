namespace FitVaga.API.DTOs;

public class AnaliseFitRequest
{
    public int CurriculoId { get; set; }
    public string DescricaoVaga { get; set; } = string.Empty;
}