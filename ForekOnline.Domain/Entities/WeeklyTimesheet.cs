// <copyright file="WeeklyTimesheet.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Purpose: Defines weekly workplace activity tracking for learner placements.

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a weekly learner timesheet submitted against a specific placement.
    /// </summary>
    [SkipAuditInterceptor]
    public class WeeklyTimesheet : Base
    {
        [Key]
        public Guid WeeklyTimesheetId { get; set; }

        public Guid PlacementId { get; set; }

        public Placement? Placement { get; set; }

        public DateTime WeekStartDate { get; set; }

        public DateTime WeekEndDate { get; set; }

        public decimal TotalHours { get; set; }

        public decimal? MondayHours { get; set; }

        public decimal? TuesdayHours { get; set; }

        public decimal? WednesdayHours { get; set; }

        public decimal? ThursdayHours { get; set; }

        public decimal? FridayHours { get; set; }

        public decimal? SaturdayHours { get; set; }

        public decimal? SundayHours { get; set; }

        public string? ActivityDescription { get; set; }

        public string? SkillsApplied { get; set; }

        public string? LearningOutcomes { get; set; }

        public string? ChallengesFaced { get; set; }

        public string? EvidenceFileName { get; set; }

        public string Status { get; set; } = "Pending Workplace Approval";

        public DateTime SubmittedOn { get; set; }

        public Guid? WorkplaceMentorDecisionBy { get; set; }

        public DateTime? WorkplaceMentorDecisionOn { get; set; }

        public string? WorkplaceMentorComments { get; set; }

        public Guid? CampusMentorAcknowledgedBy { get; set; }

        public DateTime? CampusMentorAcknowledgedOn { get; set; }

        public string? CampusMentorComments { get; set; }

        [NotMapped]
        public IFormFile? EvidenceFile { get; set; }
    }
}
