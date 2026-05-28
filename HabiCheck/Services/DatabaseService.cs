// DatabaseService.cs
// This class handles ALL reading and writing to the local database.
//
// SQLite stores your data in a single .db file on the user's computer.
// Microsoft.Data.Sqlite is the NuGet package that lets C# talk to SQLite.

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace HabiCheck.Services;

public class DatabaseService
{
    // 💡 DEVELOPER NOTE: This is the path to the database file on the user's computer.
    // Environment.GetFolderPath gives us the correct path for any Windows user.
    private static readonly string DbFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HabiCheck");

    private static readonly string DbPath = Path.Combine(DbFolder, "habicheck.db");

    // The connection string tells SQLite where the database file is.
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of DatabaseService and sets up the schema.
    /// </summary>
    public DatabaseService()
    {
        // Make sure the folder exists before trying to create the file
        Directory.CreateDirectory(DbFolder);
        _connectionString = $"Data Source={DbPath}";
        InitializeDatabase();
    }

    /// <summary>
    /// Creates the database tables if they don't exist yet.
    /// Safe to call every time the app starts — it won't delete existing data.
    /// </summary>
    private void InitializeDatabase()
    {
        // 💡 DEVELOPER NOTE: 'using var' ensures the database connection is automatically closed
        // and disposed when the method exits, releasing file locks and memory resources.
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // 'CREATE TABLE IF NOT EXISTS' means: only create if it's not already there.
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS profiles (
                id          TEXT PRIMARY KEY,
                username    TEXT NOT NULL,
                hulas_level TEXT NOT NULL DEFAULT 'Pawisin'
            );

            CREATE TABLE IF NOT EXISTS scans (
                id          TEXT PRIMARY KEY,
                profile_id  TEXT NOT NULL,
                fabric_name TEXT NOT NULL,
                grade       TEXT NOT NULL,
                fiber_type  TEXT NOT NULL,
                image_path  TEXT,
                scanned_at  TEXT NOT NULL
            );
        ";
        command.ExecuteNonQuery();
    }

    // ── Connection helper ──

    /// <summary>
    /// Opens and returns a new SQLite connection.
    /// Always use this inside a 'using' block so the connection closes automatically.
    /// </summary>
    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    // ── Profile methods (replaces Supabase auth + profiles table) ──

    /// <summary>
    /// Creates a new local user profile. Returns the new profile id (a GUID).
    /// Equivalent of Supabase signUp in the original app.
    /// </summary>
    public string CreateProfile(string username, string hulasLevel = "Pawisin")
    {
        var id = Guid.NewGuid().ToString();
        using var connection = OpenConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText =
            "INSERT INTO profiles (id, username, hulas_level) VALUES ($id, $username, $hulas)";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.Parameters.AddWithValue("$username", username);
        cmd.Parameters.AddWithValue("$hulas", hulasLevel);
        cmd.ExecuteNonQuery();
        return id;
    }

    /// <summary>
    /// Finds a profile by username. Returns null if not found.
    /// Equivalent of Supabase signInWithPassword in the original app.
    /// </summary>
    public (string Id, string Username, string HulasLevel)? GetProfileByUsername(string username)
    {
        using var connection = OpenConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, username, hulas_level FROM profiles WHERE username = $username LIMIT 1";
        cmd.Parameters.AddWithValue("$username", username);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return (reader.GetString(0), reader.GetString(1), reader.GetString(2));
        return null;
    }

    /// <summary>
    /// Updates the hulas_level for a profile.
    /// Equivalent of the Supabase profiles upsert in HabiService.SetHulasAsync().
    /// </summary>
    public void UpdateHulasLevel(string profileId, string hulasLevel)
    {
        using var connection = OpenConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE profiles SET hulas_level = $hulas WHERE id = $id";
        cmd.Parameters.AddWithValue("$hulas", hulasLevel);
        cmd.Parameters.AddWithValue("$id", profileId);
        cmd.ExecuteNonQuery();
    }

    // ── Scan methods (replaces Supabase scans table) ──

    /// <summary>
    /// Returns all scans for a profile, newest first.
    /// Equivalent of the Supabase scans query in HabiService.GetRecentScansAsync().
    /// </summary>
    public List<(string Id, string FabricName, string Grade, string FiberType, string? ImagePath, string ScannedAt)>
        GetScans(string profileId, int? limit = null)
    {
        using var connection = OpenConnection();
        var cmd = connection.CreateCommand();
        var sql = "SELECT id, fabric_name, grade, fiber_type, image_path, scanned_at " +
                  "FROM scans WHERE profile_id = $pid ORDER BY scanned_at DESC";
        if (limit.HasValue) sql += $" LIMIT {limit.Value}";
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("$pid", profileId);

        var results = new List<(string, string, string, string, string?, string)>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            results.Add((
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.GetString(5)
            ));
        }
        return results;
    }

    /// <summary>
    /// Saves a new scan record to the local database.
    /// Equivalent of inserting into the Supabase scans table.
    /// </summary>
    public void SaveScan(string profileId, string fabricName, string grade,
        string fiberType, string? imagePath)
    {
        using var connection = OpenConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO scans (id, profile_id, fabric_name, grade, fiber_type, image_path, scanned_at)
            VALUES ($id, $pid, $name, $grade, $fiber, $img, $at)";
        cmd.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("$pid", profileId);
        cmd.Parameters.AddWithValue("$name", fabricName);
        cmd.Parameters.AddWithValue("$grade", grade);
        cmd.Parameters.AddWithValue("$fiber", fiberType);
        cmd.Parameters.AddWithValue("$img", (object?)imagePath ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$at", DateTime.UtcNow.ToString("o")); // ISO 8601 format
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns a single scan by its id. Returns null if not found.
    /// </summary>
    public (string Id, string FabricName, string Grade, string FiberType, string? ImagePath, string ScannedAt)?
        GetScanById(string scanId)
    {
        using var connection = OpenConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText =
            "SELECT id, fabric_name, grade, fiber_type, image_path, scanned_at FROM scans WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", scanId);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return (reader.GetString(0), reader.GetString(1), reader.GetString(2),
                    reader.GetString(3),
                    reader.IsDBNull(4) ? null : reader.GetString(4),
                    reader.GetString(5));
        return null;
    }

    /// <summary>
    /// Deletes a scan by its id.
    /// </summary>
    public void DeleteScan(string scanId)
    {
        using var connection = OpenConnection();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM scans WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", scanId);
        cmd.ExecuteNonQuery();
    }
}
