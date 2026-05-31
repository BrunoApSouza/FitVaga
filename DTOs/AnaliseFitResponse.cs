namespace FitVaga.API.DTOs;

public class AnaliseFitResponse
{
    public int ScoreFit { get; set; }
    public string PontosFortres { get; set; } = string.Empty;
    public string Gaps { get; set; } = string.Empty;
    public string CartaApresentacao { get; set; } = string.Empty;
}