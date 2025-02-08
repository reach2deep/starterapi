using System;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for creating a new lease agreement
    /// </summary>
    public class CreateLeaseAgreementRequest
    {
        /// <summary>
        /// The ID of the unit being leased
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// The ID of the owner
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// The ID of the tenant
        /// </summary>
        public Guid TenantId { get; set; }

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
    }
} 