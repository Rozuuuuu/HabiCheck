// ScanInsight.cs
// Holds three specific AI-generated tips for a particular fabric result.

using Newtonsoft.Json;

namespace HabiCheck.Models;

public class ScanInsight
{
    /// <summary>
    /// Specialized advice for washing/maintaining this fabric to maximize lifespan.
    /// </summary>
    [JsonProperty("washTip")]
    public string WashTip { get; set; } = string.Empty;

    /// <summary>
    /// Ecological impact or recycling suggestion.
    /// </summary>
    [JsonProperty("ecoTip")]
    public string EcoTip { get; set; } = string.Empty;

    /// <summary>
    /// Comfort/suitability tip matching Cebu's current weather and humidity.
    /// </summary>
    [JsonProperty("climateTip")]
    public string ClimateTip { get; set; } = string.Empty;
}
