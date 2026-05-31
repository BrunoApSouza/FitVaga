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
        var prompt = "Você é um especialista em RH e recrutamento.\n\n" +
                    "Analise a compatibilidade entre o currículo e a vaga abaixo e retorne APENAS um JSON válido com esta estrutura, sem markdown, sem texto adicional:\n" +
                    "{\n" +
                    "  \"scoreFit\": (número de 0 a 100),\n" +
                    "  \"pontosFortres\": \"pontos fortes do candidato para esta vaga\",\n" +
                    "  \"gaps\": \"o que está faltando no perfil\",\n" +
                    "  \"cartaApresentacao\": \"carta de apresentação personalizada\"\n" +
                    "}\n\n" +
                    "CURRÍCULO:\n" + curriculo + "\n\n" +
                    "DESCRIÇÃO DA VAGA:\n" + descricaoVaga;

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

        Console.WriteLine("RESPOSTA GEMINI: " + responseString);

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
