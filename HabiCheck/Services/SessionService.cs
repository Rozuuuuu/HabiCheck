// SessionService.cs
// Holds the currently logged-in user in memory.
// This replaces Supabase's _supabase.Auth.CurrentUser.
// 'static' means there is only one instance, shared across the whole app.

namespace HabiCheck.Services;

public static class SessionService
{
    // 💡 DEVELOPER NOTE: 'static' properties belong to the class, not an object.
    // Any code anywhere can read SessionService.CurrentProfileId without
    // needing to create a SessionService instance.
    public static string? CurrentProfileId { get; set; }
    public static string? CurrentUsername { get; set; }
    public static string? CurrentHulasLevel { get; set; }

    /// <summary>Returns true if a user is currently logged in.</summary>
    public static bool IsLoggedIn => CurrentProfileId != null;

    /// <summary>Clears the session (equivalent of Supabase signOut).</summary>
    public static void Clear()
    {
        CurrentProfileId = null;
        CurrentUsername = null;
        CurrentHulasLevel = null;
    }
}
