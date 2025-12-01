using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GrammarFixer.Models;

namespace GrammarFixer.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;

        public GeminiService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<(string fixedText, int inputTokens, int outputTokens)> FixTextAsync(string text, string apiKey, Persona persona, ProcessingSpeed speed)
        {
            string prompt = GetPromptForPersona(text, persona, speed);

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
                },
                generationConfig = new
                {
                    temperature = GetTemperatureForSpeed(speed),
                    maxOutputTokens = GetMaxTokensForSpeed(speed),
                    topK = 40,
                    topP = 0.95
                }
            };

            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent?key={apiKey}";

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(url, content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Error: {response.StatusCode} - {responseText}");
            }

            var jsonDoc = JsonDocument.Parse(responseText);
            
            try
            {
                string fixedText = jsonDoc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? text;

                int inputTokens = 0;
                int outputTokens = 0;

                try
                {
                    var usageMetadata = jsonDoc.RootElement.GetProperty("usageMetadata");
                    inputTokens = usageMetadata.GetProperty("promptTokenCount").GetInt32();
                    outputTokens = usageMetadata.GetProperty("candidatesTokenCount").GetInt32();
                }
                catch
                {
                    inputTokens = prompt.Split(' ').Length;
                    outputTokens = fixedText.Split(' ').Length;
                }

                return (fixedText.Trim(), inputTokens, outputTokens);
            }
            catch (Exception)
            {
                throw new Exception("Failed to parse API response");
            }
        }

        private string GetPromptForPersona(string text, Persona persona, ProcessingSpeed speed)
        {
            string speedInstruction = speed switch
            {
                ProcessingSpeed.Fast => "Make minimal necessary corrections.",
                ProcessingSpeed.Normal => "Fix all errors thoroughly.",
                ProcessingSpeed.Detailed => "Fix all errors and improve clarity and flow.",
                _ => "Fix all errors thoroughly."
            };

            return persona switch
            {
                Persona.Friendly => $"Rewrite this text to fix all grammar, spelling, and punctuation errors while making it warm, friendly, and conversational. {speedInstruction} Only return the corrected text, nothing else:\n\n{text}",
                
                Persona.Professional => $"Rewrite this text to fix all grammar, spelling, and punctuation errors while making it formal, professional, and polished. {speedInstruction} Only return the corrected text, nothing else:\n\n{text}",
                
                Persona.Concise => $"Rewrite this text to fix all grammar, spelling, and punctuation errors while making it brief, direct, and concise. Remove unnecessary words. {speedInstruction} Only return the corrected text, nothing else:\n\n{text}",
                
                Persona.Creative => $"Rewrite this text to fix all grammar, spelling, and punctuation errors while making it expressive, engaging, and dynamic. {speedInstruction} Only return the corrected text, nothing else:\n\n{text}",
                
                _ => $"Fix all grammar, spelling, and punctuation errors in this text. Maintain the original tone and style. {speedInstruction} Only return the corrected text, nothing else:\n\n{text}"
            };
        }

        private double GetTemperatureForSpeed(ProcessingSpeed speed)
        {
            return speed switch
            {
                ProcessingSpeed.Fast => 0.1,
                ProcessingSpeed.Normal => 0.3,
                ProcessingSpeed.Detailed => 0.5,
                _ => 0.3
            };
        }

        private int GetMaxTokensForSpeed(ProcessingSpeed speed)
        {
            return speed switch
            {
                ProcessingSpeed.Fast => 512,
                ProcessingSpeed.Normal => 1024,
                ProcessingSpeed.Detailed => 2048,
                _ => 1024
            };
        }
    }
}