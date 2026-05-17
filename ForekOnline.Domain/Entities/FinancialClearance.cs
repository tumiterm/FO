// <copyright file="FinancialClearance.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    29/03/2026
// Purpose:         Defines the FinancialClearance entity.

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Tracks financial clearance status for an approved application
    /// before the applicant can proceed to enrollment.
    /// </summary>
    [Table(nameof(FinancialClearance), Schema = "Finance")]
    public class FinancialClearance : EntityBase<Guid>
    {
        /// <summary>
        /// The application this clearance is linked to.
        /// </summary>
        [ForeignKey(nameof(Application))]
        public Guid ApplicationId { get; set; }

        public Application? Application { get; set; }

        /// <summary>
        /// Current financial clearance status.
        /// </summary>
        public eFinancialClearanceStatus Status { get; set; } = eFinancialClearanceStatus.AwaitingPayment;

        /// <summary>
        /// Amount expected (informational, not enforced).
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AmountExpected { get; set; }

        /// <summary>
        /// Amount shown on proof-of-payment (captured by finance officer).
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AmountReceived { get; set; }

        /// <summary>
        /// Stored file reference for the proof-of-payment document.
        /// </summary>
        [MaxLength(500)]
        public string? ProofOfPaymentFileId { get; set; }

        /// <summary>
        /// Original filename of the uploaded proof.
        /// </summary>
        [MaxLength(250)]
        public string? ProofOfPaymentFileName { get; set; }

        /// <summary>
        /// Required when Status is Overridden. Reason for bypassing payment.
        /// Examples: "Student is NSFAS funded", "Payment arrangement confirmed", "Bursary holder".
        /// </summary>
        [MaxLength(500)]
        public string? OverrideReason { get; set; }

        /// <summary>
        /// Optional notes from the finance officer.
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// File upload (not mapped to DB).
        /// </summary>
        [NotMapped]
        public IFormFile? ProofOfPaymentFile { get; set; }
    }
}