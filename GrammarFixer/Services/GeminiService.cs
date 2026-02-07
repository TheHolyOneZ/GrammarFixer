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

        public async Task<(string fixedText, int inputTokens, int outputTokens)> FixTextAsync(string text, string apiKey, string personaInstruction, ProcessingSpeed speed, string language, bool translateToSelectedLanguage, string selectedModel)
        {
            string prompt = GetPromptForPersona(text, personaInstruction, speed, language, translateToSelectedLanguage);

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

            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{selectedModel}:generateContent?key={apiKey}";
            DebugLogService.Log($"API Request URL: {url}");

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(url, content);
            var responseText = await response.Content.ReadAsStringAsync();
            DebugLogService.Log($"API Response JSON: {responseText}");

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

        private string GetPromptForPersona(string text, string personaInstruction, ProcessingSpeed speed, string language, bool translateToSelectedLanguage)
        {
            string speedInstruction = speed switch
            {
                ProcessingSpeed.Fast => "Make minimal necessary corrections.",
                ProcessingSpeed.Normal => "Fix all errors thoroughly.",
                ProcessingSpeed.Detailed => "Fix all errors and improve clarity and flow.",
                _ => "Fix all errors thoroughly."
            };

            string languageInstruction = translateToSelectedLanguage
                ? $"Translate the following text to {language}. IMPORTANT: If {language} is not a real or valid language, output EXACTLY this message and nothing else: 'TheHolyOneZ: Whoa there! {language}? Last I checked, that’s only spoken by unicorns and coffee machines....'. Otherwise, translate and rewrite it to fix all grammar, spelling, and punctuation errors"
                : "Detect the language of the following text, and then rewrite it in the same language to fix all grammar, spelling, and punctuation errors";

            
            string combinedInstruction;
            if (translateToSelectedLanguage)
            {
                combinedInstruction = $"{languageInstruction}, and APPLY THE FOLLOWING STYLE: {personaInstruction} {speedInstruction}";
            }
            else
            {
                combinedInstruction = $"IMPORTANT: First, detect the language of the input text (let's call it 'L'). Then, rewrite the text strictly in 'L' to fix all grammar, spelling, and punctuation errors. CRITICAL: Do NOT translate to English. Your entire response must be in 'L'. While rewriting, you must adopt a specific persona. Here is the persona description (in English): '{personaInstruction}'. You are to INTERPRET this persona and apply its stylistic and tonal qualities NATURALLY and IDIOMATICALLY in language 'L'. It is essential that the persona feels native to 'L'. {speedInstruction}";
            }

            return $"{combinedInstruction} Only return the corrected text, nothing else:\n\n{text}";
        }

        public static string GetInstructionForPersona(Persona persona)
        {
            return persona switch
            {
                Persona.Friendly => "while making it warm, friendly, and conversational.",
                Persona.Professional => "while making it formal, professional, and polished.",
                Persona.Concise => "while making it brief, direct, and concise. Remove unnecessary words.",
                Persona.Creative => "while making it expressive, engaging, and dynamic.",
                _ => "while maintaining the original tone and style."
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
