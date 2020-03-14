using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Validations;

namespace TransactionApi.Server.Services
{
    public enum TransactionStatusXml
    {
        Approved = 0,
        Failed = 1,
        Finished = 2
    }
    
    public class TransactionFormatXml : IValidatableObject
    {
        public class PaymentDetailsInfo
        {
            public decimal Amount { get; set; }
            [StringLength(3, ErrorMessage = "Currency code must be 3 characters long")]
            public string CurrencyCode { get; set; }
        }
        [StringLength(50, MinimumLength = 1,ErrorMessage = "Transaction id must be at least 1 and not more than 50 characters long")]
        public string TransactionIdentificator { get; set; }
        public PaymentDetailsInfo PaymentDetails { get; set; }
        public string TransactionDate { get; set; }
        public string Status { get; set; }
        
        public Transaction ToTransactionModel()
        {
            return new Transaction()
            {
                TransactionId = this.TransactionIdentificator,
                Amount = this.PaymentDetails.Amount,
                Currency = this.PaymentDetails.CurrencyCode,
                TransactionDate = DateTime.ParseExact(this.TransactionDate, "yyyy-MM-ddThh:mm:sss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None),
                Status = ((TransactionStatusXml)Enum.Parse(typeof(TransactionStatusXml), this.Status)).ToModelStatus()
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (!ValidationHelper.IsValidXmlDate(this.TransactionDate))
            {
                results.Add(new ValidationResult("Date must be in correct format."));
            }
            if (!ValidationHelper.IsValidCurrency(this.PaymentDetails.CurrencyCode))
            {
                results.Add(new ValidationResult("Currency code is not of ISO4217 format"));
            }

            if (!ValidationHelper.IsValidXmlStatus(this.Status))
            {
                results.Add((new ValidationResult("Invalid transaction status")));
            }
            
            return results;

            
        }
    }
}