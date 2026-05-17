// <copyright file="WeeklyTimesheetViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Purpose: Defines the weekly timesheet form/view model.

#region Usings
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Captures weekly learner activity and approval workflow details for a placement.
    /// </summary>
    public class WeeklyTimesheetViewModel
    {
        public Guid WeeklyTimesheetId { get; set; }

        [Required]
        public Guid PlacementId { get; set; }

        [Display(Name = "Week Start")]
        public DateTime WeekStartDate { get; set; }

        [Display(Name = "Week End")]
        public DateTime WeekEndDate { get; set; }

        [Required]
        [Range(0.1, 168)]
        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        public decimal? MondayHours { get; set; }

        public decimal? TuesdayHours { get; set; }

        public decimal? WednesdayHours { get; set; }

        public decimal? ThursdayHours { get; set; }

        public decimal? FridayHours { get; set; }

        public decimal? SaturdayHours { get; set; }

        public decimal? SundayHours { get; set; }

        [Required]
        [Display(Name = "Activity Description / Tasks Performed")]
        public string? ActivityDescription { get; set; }

        [Display(Name = "Skills Applied")]
        public string? SkillsApplied { get; set; }

        [Display(Name = "Learning Outcomes Achieved")]
        public string? LearningOutcomes { get; set; }

        [Display(Name = "Challenges Faced")]
        public string? ChallengesFaced { get; set; }

        [Display(Name = "Evidence Upload")]
        public IFormFile? EvidenceFile { get; set; }

        public string? EvidenceFileName { get; set; }

        public string Status { get; set; } = "Pending Workplace Approval";

        public DateTime SubmittedOn { get; set; }

        public string? WorkplaceMentorComments { get; set; }

        public string? CampusMentorComments { get; set; }
    }
}
