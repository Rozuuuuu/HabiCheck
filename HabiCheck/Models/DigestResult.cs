// DigestResult.cs
// Holds the AI-generated digest of the user's scanning history.

using Newtonsoft.Json;

namespace HabiCheck.Models;

public class DigestResult
{
    /// <summary>
    /// Narrative summary of the user's scanning habits.
    /// </summary>
    [JsonProperty("summary")]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Average ecological rating calculated from scans (0 to 100).
    /// </summary>
    [JsonProperty("ecoScore")]
    public int EcoScore { get; set; }

    /// <summary>
    /// Most frequently scanned fabric name.
    /// </summary>
    [JsonProperty("topFabric")]
    public string TopFabric { get; set; } = string.Empty;

    /// <summary>
    /// Actionable ecological and lifestyle recommendations.
    /// </summary>
    [JsonProperty("recommendation")]
    public string Recommendation { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of when the digest was generated.
    /// </summary>
    [JsonProperty("generatedAt")]
    public string GeneratedAt { get; set; } = string.Empty;
}
