// ScanRecord.cs
// This represents a single fabric scan record saved in the database.
// All fields map directly to columns in the SQLite 'scans' table.

using Newtonsoft.Json;

namespace HabiCheck.Models;

public class ScanRecord
{
    /// <summary>
    /// Unique identifier for the scan (GUID).
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The local profile ID of the user who performed this scan.
    /// </summary>
    public string ProfileId { get; set; } = string.Empty;

    /// <summary>
    /// Friendly name of the fabric (e.g. Linen, Polyester).
    /// </summary>
    public string FabricName { get; set; } = string.Empty;

    /// <summary>
    /// Eco-grade of the fabric (e.g., A+, F-).
    /// </summary>
    public string Grade { get; set; } = string.Empty;

    /// <summary>
    /// Composition details (e.g., 100% Linen).
    /// </summary>
    public string FiberType { get; set; } = string.Empty;

    /// <summary>
    /// Path to the local image file stored in the app data folder.
    /// </summary>
    public string? ImagePath { get; set; }

    /// <summary>
    /// ISO 8601 timestamp of when the scan occurred.
    /// </summary>
    public string ScannedAt { get; set; } = string.Empty;

    /// <summary>
    /// Temporary image data (Base64 URL) for offline drafts.
    /// </summary>
    [JsonIgnore]
    public string? ImageDataUrl { get; set; }

    /// <summary>
    /// Checks if this is an offline draft.
    /// </summary>
    public bool IsOffline => Id.StartsWith("offline:");
}
