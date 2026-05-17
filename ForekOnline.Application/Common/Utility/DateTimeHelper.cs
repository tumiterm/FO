// <copyright file="DateTimeHelper.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-03-2026 21:24 PM
// Purpose:         Defines the DateTimeHelper.

/// <summary>
/// Provides utility methods for working with date and time values, including conversion to South African Standard Time
/// (SAST, UTC+2).
/// </summary>
/// <remarks>This static class is intended for scenarios where consistent handling of SAST time zone is required
/// across platforms. All methods are thread-safe and do not maintain any internal state.</remarks>
public static class DateTimeHelper
{
    /// <summary>
    /// Gets the current DateTimeOffset in South African Standard Time (SAST, UTC+2).
    /// This uses the IANA time zone ID "Africa/Johannesburg" for cross-platform compatibility.
    /// </summary>
    /// <returns>The current DateTimeOffset in SAST.</returns>
    public static DateTimeOffset GetCurrentSastDateTimeOffset()
    {
        try
        {
            var sastTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Johannesburg");
            return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, sastTimeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(2));
        }
    }
}