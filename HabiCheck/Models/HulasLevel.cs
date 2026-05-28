// HulasLevel.cs
// Defines the climate perspiration levels of Cebu residents to tailor fabric recommendations.

namespace HabiCheck.Models;

public enum HulasLevel
{
    /// <summary>
    /// Person sweats a lot; requires highly breathable natural fibers (linen, cotton) and must avoid polyester.
    /// </summary>
    Pawisin,

    /// <summary>
    /// Normal perspiration profile; suits cotton/bamboo blends.
    /// </summary>
    Normal,

    /// <summary>
    /// Person rarely sweats; comfortable in a wider variety of blends.
    /// </summary>
    Chill
}
