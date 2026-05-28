// FabricData.cs
// Holds descriptive information and eco-analysis results for a scanned fabric type.

using System.Collections.Generic;

namespace HabiCheck.Models;

public class FabricData
{
    /// <summary>
    /// Friendly name of the fabric.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Eco-grade of the fabric (e.g. A+, F-).
    /// </summary>
    public string Grade { get; set; } = string.Empty;

    /// <summary>
    /// Details about the fibers (e.g. 100% Cotton).
    /// </summary>
    public string FiberType { get; set; } = string.Empty;

    /// <summary>
    /// Breathability score from 0 (lowest) to 100 (highest).
    /// </summary>
    public int Breathability { get; set; }

    /// <summary>
    /// Sustainability score from 0 (worst) to 100 (best).
    /// </summary>
    public int Sustainability { get; set; }

    /// <summary>
    /// General description of the fabric's characteristics.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Personal climate recommendation based on the user's hulas profile.
    /// </summary>
    public string PersonalMessage { get; set; } = string.Empty;

    /// <summary>
    /// Warning message if the fabric is unsuitable for tropical climates.
    /// </summary>
    public string? ClimateAlert { get; set; }

    /// <summary>
    /// Bulleted washing and maintenance guidelines.
    /// </summary>
    public List<string> WashTips { get; set; } = new();

    /// <summary>
    /// Estimated resale price range.
    /// </summary>
    public string ResaleValue { get; set; } = string.Empty;

    /// <summary>
    /// Creative idea for recycling or upcycling the fabric.
    /// </summary>
    public string UpcyclingIdea { get; set; } = string.Empty;

    /// <summary>
    /// True if the fabric is natural and eco-friendly (grade A).
    /// </summary>
    public bool IsSuccess { get; set; }
}
