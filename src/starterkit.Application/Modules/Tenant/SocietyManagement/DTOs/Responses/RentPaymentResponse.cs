using System;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses
{
    /// <summary>
    /// DTO for rent payment response
    /// </summary>
    public class RentPaymentResponse
    {
        /// <summary>
        /// The unique identifier of the rent payment
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The ID of the lease agreement this payment belongs to
        /// </summary>
        public Guid LeaseAgreementId { get; set; }

        /// <summary>
        /// The payment amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The due date for the payment
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The actual date when payment was made
        /// </summary>
        public DateTime? PaidDate { get; set; }

        /// <summary>
        /// The payment mode (e.g., Cash, Online, Check)
        /// </summary>
        public string PaymentMode { get; set; }

        /// <summary>
        /// The transaction reference number
        /// </summary>
        public string TransactionReference { get; set; }

        /// <summary>
        /// The status of the payment
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The date when the payment record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The ID of the user who created the payment record
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// The date when the payment record was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The ID of the user who last updated the payment record
        /// </summary>
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        /// Basic lease agreement information
        /// </summary>
        public LeaseAgreementBasicInfo LeaseAgreement { get; set; }
    }

    /// <summary>
    /// Basic lease agreement information for rent payment responses
    /// </summary>
    public class LeaseAgreementBasicInfo
    {
        /// <summary>
        /// The unique identifier of the lease agreement
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The unit number
        /// </summary>
        public string UnitNumber { get; set; }

        /// <summary>
        /// The tenant's full name
        /// </summary>
        public string TenantName { get; set; }
    }
} 