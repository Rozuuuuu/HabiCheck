// HabiService translates the functions from habi.ts into C# methods.
// All methods that do slow work use 'async Task' —
// this is like JavaScript's async/await. 'await' pauses the method
// until the slow work is done, without freezing the window.

using HabiCheck.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HabiCheck.Services;

public class HabiService
{
    private readonly DatabaseService _db;

    // 💡 DEVELOPER NOTE: 'readonly' means this field can only be set once, in the constructor.
    public HabiService(DatabaseService db)
    {
        _db = db;
    }

    // --- HULAS LEVEL ---

    /// <summary>
    /// Gets the stored hulas level from the local user session.
    /// Equivalent of getHulas() in habi.ts.
    /// </summary>
    public HulasLevel GetHulas()
    {
        var stored = SessionService.CurrentHulasLevel ?? "Pawisin";
        // 💡 DEVELOPER NOTE: Enum.TryParse converts a string like "Pawisin" to
        // the HulasLevel.Pawisin enum value. The 'out var level' captures the result.
        return Enum.TryParse<HulasLevel>(stored, out var level) ? level : HulasLevel.Pawisin;
    }

    /// <summary>
    /// Saves the hulas level to the local database and updates the session.
    /// Equivalent of setHulas() in habi.ts.
    /// </summary>
    public void SetHulas(HulasLevel value)
    {
        var profileId = SessionService.CurrentProfileId;
        if (profileId != null)
        {
            _db.UpdateHulasLevel(profileId, value.ToString());
            SessionService.CurrentHulasLevel = value.ToString();
        }
    }

    // --- SCAN DATA ---

    /// <summary>
    /// Gets the most recent scans from the local database.
    /// Equivalent of getRecentScans() in habi.ts.
    /// </summary>
    public List<ScanRecord> GetRecentScans(int? limit = null)
    {
        var profileId = SessionService.CurrentProfileId ?? string.Empty;
        var rows = _db.GetScans(profileId, limit);
        // 💡 DEVELOPER NOTE: .Select() transforms each item in a list — like .map() in JavaScript.
        // .ToList() executes the query and converts the sequence to a list.
        return rows.Select(r => new ScanRecord
        {
            Id = r.Id,
            ProfileId = profileId,
            FabricName = r.FabricName,
            Grade = r.Grade,
            FiberType = r.FiberType,
            ScannedAt = r.ScannedAt,
            ImagePath = r.ImagePath
        }).ToList();
    }

    /// <summary>
    /// Gets a single scan by its id from the local database.
    /// Returns null if not found.
    /// </summary>
    public ScanRecord? GetScanById(string scanId)
    {
        var row = _db.GetScanById(scanId);
        // 💡 DEVELOPER NOTE: '?.' is the null-conditional operator — it only calls
        // the code on the right if 'row' is not null. Safe way to avoid crashes.
        if (row == null) return null;
        return new ScanRecord
        {
            Id = row.Value.Id,
            ProfileId = SessionService.CurrentProfileId ?? string.Empty,
            FabricName = row.Value.FabricName,
            Grade = row.Value.Grade,
            FiberType = row.Value.FiberType,
            ScannedAt = row.Value.ScannedAt,
            ImagePath = row.Value.ImagePath
        };
    }

    /// <summary>
    /// Returns the local file path for a scan image.
    /// Images are stored in the HabiCheck app data folder.
    /// </summary>
    public string? GetScanImagePath(string? path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HabiCheck", "Images");
        return Path.Combine(folder, path);
    }

    // --- FABRIC RESULT BUILDER ---

    /// <summary>
    /// Builds fabric result data based on success or failure.
    /// Equivalent of buildFabricResult() in habi.ts.
    /// </summary>
    public FabricData BuildFabricResult(bool isSuccess)
    {
        var hulas = GetHulas();
        if (isSuccess)
        {
            return new FabricData
            {
                Name = "Premium Linen",
                Grade = "A+",
                FiberType = "100% Natural Linen",
                Breathability = 95,
                Sustainability = 90,
                PersonalMessage = hulas == HulasLevel.Pawisin
                    ? "Perfect! Super breathable ito para sa Cebu humidity. Goodbye sticky feeling!"
                    : "Great choice! This natural fabric will keep you comfortable all day.",
                WashTips = new List<string> { "Cold water wash only", "Hang dry in shade", "Iron while damp" },
                ResaleValue = "₱450 – ₱850",
                UpcyclingIdea = "Reusable market bags or decorative pillow covers.",
                IsSuccess = true
            };
        }
        return new FabricData
        {
            Name = "Polyester Blend",
            Grade = "F-",
            FiberType = "85% Polyester, 15% Rayon",
            Breathability = 25,
            Sustainability = 15,
            PersonalMessage = hulas == HulasLevel.Pawisin
                ? "Babala! Plastic bag ang feel nito sa init. Mataas ang risk ng amoy-araw!"
                : "Not ideal for our tropical climate. Poor airflow and moisture-wicking.",
            ClimateAlert = "Sa 32°C ng Cebu, itrap ng fabric na ito ang sweat at magiging amoy-araw ka bago mag-tanghali.",
            WashTips = new List<string> { "Wash separately", "Use fabric softener", "Low heat or air dry" },
            ResaleValue = "₱80 – ₱150",
            UpcyclingIdea = "\"Basahan\" – cleaning cloth for floors or windows.",
            IsSuccess = false
        };
    }

    /// <summary>
    /// Returns the label and advice text for a given hulas level.
    /// Equivalent of getHulasPersona() in habi.ts.
    /// </summary>
    public (string Label, string Advice) GetHulasPersona(HulasLevel hulas)
    {
        // 💡 DEVELOPER NOTE: This is a C# 'switch expression' — cleaner than if/else
        // for multiple cases.
        return hulas switch
        {
            HulasLevel.Chill => ("Chill Lang Profile", "You stay cool naturally! Cotton or linen blends work great for you."),
            HulasLevel.Normal => ("Normal Lang Profile", "Breathable cotton or bamboo blends are your best friend."),
            _ => ("Pawisin Profile", "Sa 32°C ng Cebu, stick to 100% linen or cotton. Iwasan ang polyester!")
        };
    }

    /// <summary>
    /// Returns hardcoded weather data for Cebu.
    /// Equivalent of getWeather() in habi.ts.
    /// </summary>
    public async Task<WeatherInfo> GetWeatherAsync()
    {
        // 💡 DEVELOPER NOTE: 'async Task' marks this method as asynchronous.
        // 'await Task.Delay' pauses without freezing the window.
        await Task.Delay(400);
        return new WeatherInfo
        {
            Location = "Consolacion, Cebu",
            Temperature = 32,
            FeelsLike = 37,
            Humidity = 85,
            WindSpeed = 12,
            Condition = "Sunny"
        };
    }

    public string HumidityLabel(int h) =>
        h >= 80 ? "High Humidity" : h >= 60 ? "Moderate Humidity" : "Low Humidity";

    public string FabricAdvice(int h) =>
        h >= 80 ? "Wear breathable natural fabrics today!" : "Any breathable fabric works fine today.";
}
