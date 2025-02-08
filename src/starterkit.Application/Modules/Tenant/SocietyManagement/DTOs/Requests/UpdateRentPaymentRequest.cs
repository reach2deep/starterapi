using System;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for updating an existing rent payment
    /// </summary>
    public class UpdateRentPaymentRequest
    {
        /// <summary>
        /// The unique identifier of the rent payment
        /// </summary>
        public Guid Id { get; set; }

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
    }
} 