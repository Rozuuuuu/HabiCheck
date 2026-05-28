// AntigravityService.cs
// This service communicates with the Google Gemini API using C# HttpClient.
// It handles all three AI-powered features: chat, scan insights, and weekly digests.
//
// 💡 DEVELOPER NOTE: 'async' and 'await' allow the service to wait for the web API response
// without blocking the user interface thread, preventing the application from freezing.

using HabiCheck.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HabiCheck.Services;

public class AntigravityService
{
    private readonly HttpClient _http;
    private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    /// <summary>
    /// Initializes a new instance of AntigravityService.
    /// </summary>
    public AntigravityService(HttpClient http)
    {
        _http = http;
        
        // Add the Gemini API header to every request made by this HttpClient instance.
        _http.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", GetApiKey());
    }

    private static string GetApiKey()
    {
        // 💡 DEVELOPER NOTE: We look for config.txt in:
        // 1. Current runtime folders and all parent directories (to find it in repository root).
        // 2. The system LocalAppData folder.
        var searchPaths = new List<string>();

        // Add AppData path
        searchPaths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HabiCheck", "config.txt"));

        // Traverse up from BaseDirectory (binary folder)
        var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (dir != null)
        {
            searchPaths.Add(Path.Combine(dir.FullName, "config.txt"));
            dir = dir.Parent;
        }

        // Traverse up from Current Working Directory
        dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir != null)
        {
            searchPaths.Add(Path.Combine(dir.FullName, "config.txt"));
            dir = dir.Parent;
        }

        foreach (var path in searchPaths.Distinct())
        {
            if (File.Exists(path))
            {
                try
                {
                    string content = File.ReadAllText(path).Trim();
                    if (string.IsNullOrEmpty(content)) continue;

                    // If file contains KEY=value (e.g. GEMINI_API_KEY="key")
                    if (content.Contains('='))
                    {
                        var parts = content.Split(new[] { '=' }, 2);
                        content = parts[1].Trim();
                    }

                    // Strip any leading/trailing quotes
                    content = content.Trim('"', '\'');

                    // 💡 DIAGNOSTIC: Print key details to Output window to verify correct parsing
                    System.Diagnostics.Debug.WriteLine($"[HabiCheck Auth] Loaded key from: {path}");
                    System.Diagnostics.Debug.WriteLine($"[HabiCheck Auth] Key length: {content.Length} characters");
                    if (content.Length > 10)
                    {
                        System.Diagnostics.Debug.WriteLine($"[HabiCheck Auth] Key preview: {content.Substring(0, 6)}...{content.Substring(content.Length - 4)}");
                    }

                    return content;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error reading key from {path}: {ex.Message}");
                }
            }
        }

        // Return placeholder for development; user needs to replace this to use AI features.
        return "YOUR_GEMINI_API_KEY_HERE";
    }

    // ── Helper to Call Gemini API ──

    /// <summary>
    /// Sends a generate content request to the Gemini API and returns the text response.
    /// </summary>
    private async Task<string> CallGeminiAsync(string systemPrompt, List<FabricChatMessage> messages)
    {
        var apiKey = GetApiKey();
        var requestUrl = $"{ApiUrl}?key={apiKey}";

        // Build the payload expected by the Google Gemini API:
        var payload = new
        {
            contents = messages.Select(m => new
            {
                role = m.Role.ToLower() == "assistant" || m.Role.ToLower() == "model" ? "model" : "user",
                parts = new[]
                {
                    new { text = m.Content }
                }
            }).ToArray(),
            systemInstruction = new
            {
                parts = new[]
                {
                    new { text = systemPrompt }
                }
            },
            generationConfig = new
            {
                temperature = 0.2,
                maxOutputTokens = 1024
            }
        };

        var jsonPayload = JsonConvert.SerializeObject(payload);
        using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // 💡 DEVELOPER NOTE: SendAsync sends the POST request to the server.
        // 'await' waits for the response asynchronously so the app stays responsive.
        var response = await _http.PostAsync(requestUrl, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Gemini API error (Status {(int)response.StatusCode}): {errBody}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        
        // Parse the response to extract candidate's text content:
        var responseObj = JsonConvert.DeserializeObject<JObject>(responseJson);
        var text = responseObj?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

        return text ?? string.Empty;
    }

    /// <summary>
    /// Strips markdown code fences (e.g., ```json ... ```) from an AI response.
    /// Necessary because models often wrap JSON code blocks.
    /// </summary>
    public string StripCodeFences(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        text = text.Trim();
        
        // Check if starts with ```
        if (text.StartsWith("```"))
        {
            // Find the line break after the fence (e.g. ```json\n)
            int index = text.IndexOf('\n');
            if (index != -1)
            {
                text = text.Substring(index + 1);
            }
            else
            {
                text = text.Substring(3);
            }
            
            // Check if ends with ```
            if (text.EndsWith("```"))
            {
                text = text.Substring(0, text.Length - 3);
            }
        }
        return text.Trim();
    }

    // ── Antigravity Feature 1: Chat Drawer ──

    /// <summary>
    /// Sends a chat message to Gemini to discuss fabric properties and Cebu climate.
    /// </summary>
    public async Task<string> SendFabricChatMessageAsync(string scanRecordJson, List<FabricChatMessage> chatHistory, string newMsg, string hulasPersona)
    {
        var systemPrompt = $"You are Antigravity, an eco-conscious fabric analysis chatbot for HabiCheck.\n" +
            $"The user has scanned a fabric. Here are the details of the scan:\n" +
            $"{scanRecordJson}\n\n" +
            $"The user's climate/perspiration profile is:\n" +
            $"{hulasPersona}\n\n" +
            $"Focus on how this fabric reacts in Cebu's high temperature (32°C) and high humidity (85%).\n" +
            $"Be helpful, conversational, and direct. Keep your answers concise, practical, and localized to Cebu's tropical realities (such as 'amoy-araw' warnings, airflow, and sticky humidity comfort).";

        // Create a copy of the history to modify
        var messages = new List<FabricChatMessage>(chatHistory);
        messages.Add(new FabricChatMessage("user", newMsg));

        return await CallGeminiAsync(systemPrompt, messages);
    }

    // ── Antigravity Feature 2: Result Insights ──

    /// <summary>
    /// Generates three specialized tips for the Result window based on fabric, weather, and hulas profile.
    /// </summary>
    public async Task<ScanInsight> GenerateScanInsightAsync(string scanResultJson, string weatherJson, string hulasPersona)
    {
        var systemPrompt = $"You are Antigravity, an eco-conscious fabric analysis engine.\n" +
            $"Generate three specialized tips based on the fabric scan, Cebu's current weather, and the user's perspiration profile.\n" +
            $"Inputs:\n" +
            $"- Fabric Info: {scanResultJson}\n" +
            $"- Weather Info: {weatherJson}\n" +
            $"- Perspiration Profile: {hulasPersona}\n\n" +
            $"You MUST return your response as a raw JSON object matching the following structure. Do NOT include any intro, explanation, or conversational text. Return ONLY the JSON:\n" +
            $"{{\n" +
            $"  \"washTip\": \"Specialized advice on washing/cleaning to prolong fiber life (e.g. cold water, mild detergents, air-drying).\",\n" +
            $"  \"ecoTip\": \"An ecological advice or upcycling suggestion tailored to this fiber type.\",\n" +
            $"  \"climateTip\": \"Climate-specific wear comfort and performance tips for Cebu's high heat and humidity.\"\n" +
            $"}}";

        var dummyHistory = new List<FabricChatMessage>
        {
            new FabricChatMessage("user", "Generate insights for this scan.")
        };

        var rawResponse = await CallGeminiAsync(systemPrompt, dummyHistory);
        var cleanedResponse = StripCodeFences(rawResponse);

        return JsonConvert.DeserializeObject<ScanInsight>(cleanedResponse) ?? new ScanInsight();
    }

    // ── Antigravity Feature 3: History Digest ──

    /// <summary>
    /// Analyzes the user's scan history to calculate an ecological score, common fabric, and weekly summary/advice.
    /// </summary>
    public async Task<DigestResult> GenerateHistoryDigestAsync(string scansJson, string hulasPersona)
    {
        var systemPrompt = $"You are Antigravity, an eco-fabric auditor.\n" +
            $"Analyze the user's history of scanned fabrics and generate a weekly eco-digest.\n" +
            $"Inputs:\n" +
            $"- Scan History: {scansJson}\n" +
            $"- Perspiration Profile: {hulasPersona}\n\n" +
            $"Perform an analysis and calculate:\n" +
            $"1. An eco score (0 to 100) based on how many natural/sustainable fibers are scanned versus synthetics (like polyester).\n" +
            $"2. The top fabric scanned.\n" +
            $"3. A concise summary of their choices.\n" +
            $"4. A primary recommendation for upgrading their wardrobe for ecological and Cebu climate suitability.\n\n" +
            $"You MUST return your response as a raw JSON object matching the following structure. Do NOT include any conversational intro or other text. Return ONLY the JSON:\n" +
            $"{{\n" +
            $"  \"summary\": \"Concise review of their overall scanning habits (1-2 sentences).\",\n" +
            $"  \"ecoScore\": 75,\n" +
            $"  \"topFabric\": \"Linen\",\n" +
            $"  \"recommendation\": \" wardrobe upgrade suggestion based on their hulas profile (1-2 sentences).\",\n" +
            $"  \"generatedAt\": \"{DateTime.UtcNow:o}\"\n" +
            $"}}";

        var dummyHistory = new List<FabricChatMessage>
        {
            new FabricChatMessage("user", "Generate a weekly digest of my scans.")
        };

        var rawResponse = await CallGeminiAsync(systemPrompt, dummyHistory);
        var cleanedResponse = StripCodeFences(rawResponse);

        var result = JsonConvert.DeserializeObject<DigestResult>(cleanedResponse);
        if (result != null)
        {
            result.GeneratedAt = DateTime.Now.ToString("g"); // Localized date/time format
            return result;
        }

        return new DigestResult
        {
            Summary = "Unable to parse digest.",
            Recommendation = "Please try again later.",
            GeneratedAt = DateTime.Now.ToString("g")
        };
    }
}
