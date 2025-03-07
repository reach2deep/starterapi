using System;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for creating a new rent payment by unit ID
    /// </summary>
    public class CreateRentPaymentByUnitRequest
    {
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
    }
} 