using System;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for updating an existing lease agreement
    /// </summary>
    public class UpdateLeaseAgreementRequest
    {
        /// <summary>
        /// The unique identifier of the lease agreement
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The end date of the lease
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The monthly rent amount
        /// </summary>
        public decimal RentAmount { get; set; }

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
    }
} 