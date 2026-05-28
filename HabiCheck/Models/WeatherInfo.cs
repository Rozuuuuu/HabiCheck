// WeatherInfo.cs
// Holds real-time or simulated local weather details for location-specific advice.

namespace HabiCheck.Models;

public class WeatherInfo
{
    /// <summary>
    /// Location name (e.g. Consolacion, Cebu).
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Current temperature in Celsius.
    /// </summary>
    public int Temperature { get; set; }

    /// <summary>
    /// Heat index / "Feels Like" temperature in Celsius.
    /// </summary>
    public int FeelsLike { get; set; }

    /// <summary>
    /// Atmospheric humidity percentage (0-100).
    /// </summary>
    public int Humidity { get; set; }

    /// <summary>
    /// Wind speed in km/h.
    /// </summary>
    public int WindSpeed { get; set; }

    /// <summary>
    /// Short description of weather condition (e.g. Sunny, Rainy).
    /// </summary>
    public string Condition { get; set; } = string.Empty;
}
