using System;
using System.Collections.Generic;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses
{
    /// <summary>
    /// DTO for lease agreement response
    /// </summary>
    public class LeaseAgreementResponse
    {
        /// <summary>
        /// The unique identifier of the lease agreement
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The ID of the unit being leased
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// The unit number
        /// </summary>
        public string UnitNumber { get; set; }

        /// <summary>
        /// The ID of the owner
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// The owner's full name
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// The ID of the tenant
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// The tenant's full name
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// The start date of the lease
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the lease
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The monthly rent amount
        /// </summary>
        public decimal RentAmount { get; set; }

        /// <summary>
        /// The security deposit amount
        /// </summary>
        public decimal SecurityDeposit { get; set; }

        /// <summary>
        /// The payment frequency (e.g., Monthly, Quarterly, Yearly)
        /// </summary>
        public string PaymentFrequency { get; set; }

        /// <summary>
        /// The notice period in days
        /// </summary>
        public int NoticePeriodDays { get; set; }

        /// <summary>
        /// The status of the lease agreement
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The date when the lease agreement was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The ID of the user who created the lease agreement
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// The date when the lease agreement was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The ID of the user who last updated the lease agreement
        /// </summary>
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        /// Indicates if the lease is nearing its end date (within 30 days)
        /// </summary>
        public bool IsExpiringSoon { get; set; }

        /// <summary>
        /// Number of days remaining until the lease expires
        /// </summary>
        public int DaysUntilExpiry { get; set; }

        /// <summary>
        /// Total amount of rent paid so far under this lease
        /// </summary>
        public decimal TotalPaidAmount { get; set; }

        /// <summary>
        /// Total pending rent amount for this lease
        /// </summary>
        public decimal PendingAmount { get; set; }

        /// <summary>
        /// Date of the last rent payment made
        /// </summary>
        public DateTime? LastPaymentDate { get; set; }

        /// <summary>
        /// Status of the last rent payment
        /// </summary>
        public string LastPaymentStatus { get; set; }

        /// <summary>
        /// List of rent payments associated with this lease
        /// </summary>
        public ICollection<RentPaymentResponse> RentPayments { get; set; }
    }
} 