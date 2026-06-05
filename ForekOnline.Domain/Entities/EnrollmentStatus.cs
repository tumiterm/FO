// <copyright file="EnrollmentStatus.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Purpose:         Defines valid enrollment statuses and external-value normalization.

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Provides the status values accepted by the Academics.EnrollmentHistory check constraint.
    /// </summary>
    public static class EnrollmentStatus
    {
        public const string Active = "Active";
        public const string Completed = "Completed";
        public const string DroppedOut = "Dropped Out";
        public const string Suspended = "Suspended";

        /// <summary>
        /// Maps status text supplied by external student sources to a value accepted by
        /// <c>CK_EnrollmentHistory_Status</c>. Unknown values use the enrollment state instead
        /// of being sent to SQL Server unchanged.
        /// </summary>
        public static string Normalize(string? status, bool? isActive, DateTime? dateCompleted)
        {
            var normalized = status?.Trim();

            if (Matches(normalized, Active, "In Progress", "Enrolled", "Registered", "Current", "Started"))
                return Active;

            if (Matches(normalized, Completed, "Complete", "Graduated"))
                return Completed;

            if (Matches(normalized, DroppedOut, "DroppedOut", "Withdrawn", "Deregistered", "Cancelled", "Canceled"))
                return DroppedOut;

            if (Matches(normalized, Suspended))
                return Suspended;

            if (dateCompleted.HasValue)
                return Completed;

            return isActive is false ? DroppedOut : Active;
        }

        private static bool Matches(string? value, params string[] candidates)
        {
            return candidates.Any(candidate =>
                string.Equals(value, candidate, StringComparison.OrdinalIgnoreCase));
        }
    }
}
