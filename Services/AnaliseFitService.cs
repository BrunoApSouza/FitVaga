using System.Text;
using System.Text.Json;
using FitVaga.API.DTOs;

namespace FitVaga.API.Services;

public class AnaliseFitService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AnaliseFitService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? "";
    }

    public async Task<AnaliseFitResponse> AnalisarFit(string curriculo, string descricaoVaga)
    {
        var prompt = "Voc� � um especialista em RH e recrutamento.\n\n" +
                    "Analise a compatibilidade entre o curr�culo e a vaga abaixo e retorne APENAS um JSON v�lido com esta estrutura, sem markdown, sem texto adicional:\n" +
                    "{\n" +
                    "  \"scoreFit\": (n�mero de 0 a 100),\n" +
                    "  \"pontosFortes\": \"pontos fortes do candidato para esta vaga\",\n" +
                    "  \"gaps\": \"o que est� faltando no perfil\",\n" +
                    "  \"cartaApresentacao\": \"carta de apresenta��o personalizada\"\n" +
                    "}\n\n" +
                    "CURR�CULO:\n" + curriculo + "\n\n" +
                    "DESCRI��O DA VAGA:\n" + descricaoVaga;

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

        var response = await _httpClient.PostAsync(url, content);
        var responseString = await response.Content.ReadAsStringAsync();

        var responseJson = JsonDocument.Parse(responseString);

        // Verifica se teve erro na resposta do Gemini
        if (responseJson.RootElement.TryGetProperty("error", out var error))
        {
            throw new Exception($"Erro Gemini: {error.GetProperty("message").GetString()}");
        }

        var textContent = responseJson
            .RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "";

        // Remove markdown se vier com ```json
        textContent = textContent
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        var resultado = JsonSerializer.Deserialize<AnaliseFitResponse>(textContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return resultado ?? new AnaliseFitResponse(); 
    }
}
