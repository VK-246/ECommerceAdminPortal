using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services;

public class GeminiAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini ApiKey is missing");
    }

    public async Task<string> GenerateProductDescriptionAsync(string productName, string? categoryName, string? additionalSpecs)
    {
        var prompt = $"Write a compelling, SEO-friendly product description for a product named '{productName}'.";
        
        if (!string.IsNullOrEmpty(categoryName))
        {
            prompt += $" It belongs to the '{categoryName}' category.";
        }
        
        if (!string.IsNullOrEmpty(additionalSpecs))
        {
            prompt += $" Here are some additional details: {additionalSpecs}.";
        }
        
        prompt += " Keep it under 3 paragraphs, engaging, and ready for an e-commerce storefront. Do not use markdown formatting like **bold**, just plain text.";

        return await CallGeminiAsync(prompt);
    }

    public async Task<string> GetMarketingAdviceAsync(string prompt)
    {
        var contextPrompt = $"You are an expert E-Commerce marketing advisor. A store manager is asking for your advice: {prompt}";
        return await CallGeminiAsync(contextPrompt);
    }

    private async Task<string> CallGeminiAsync(string prompt)
    {
        return await ExecuteWithRetryAsync(() => CallGeminiInternalAsync(prompt));
    }

    private async Task<string> CallGeminiInternalAsync(string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={_apiKey}";

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

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API error: {error}");
        }

        var responseData = await response.Content.ReadFromJsonAsync<GeminiResponse>();

        var generatedText = responseData?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        if (string.IsNullOrEmpty(generatedText))
        {
            throw new Exception("Gemini returned an empty response.");
        }

        return generatedText.Trim();
    }

    private async Task<string> ExecuteWithRetryAsync(Func<Task<string>> apiCall, int maxRetries = 3)
    {
        int delayMs = 1000;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await apiCall();
            }
            catch (Exception ex) when (ex.Message.Contains("503") || ex.Message.Contains("UNAVAILABLE"))
            {
                if (attempt == maxRetries) 
                    throw; // Re-throw if max retries reached

                await Task.Delay(delayMs);
                delayMs *= 2; // Double delay: 1s, 2s, 4s
            }
        }

        throw new InvalidOperationException("Request failed after maximum retries.");
    }

    // Classes to deserialize the Gemini API response
    private class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public Candidate[]? Candidates { get; set; }
    }

    private class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }
    }

    private class Content
    {
        [JsonPropertyName("parts")]
        public Part[]? Parts { get; set; }
    }

    private class Part
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
