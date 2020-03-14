using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Validations;
using TransactionApi.Shared.Enums;

namespace TransactionApi.Server.Services
{
    public enum TransactionStatusCsv
    {
        Approved = 0,
        Rejected = 1,
        Done = 2
    }

    public static class TransactionStatusExtensions
    {
        public static TransactionStatus ToModelStatus(this TransactionStatusCsv status)
        {
            return status switch
            {
                TransactionStatusCsv.Approved => TransactionStatus.A,
                TransactionStatusCsv.Rejected => TransactionStatus.R,
                TransactionStatusCsv.Done => TransactionStatus.D,
                _ => throw new InvalidOperationException("Incorrect csv status")
            };
        }
        public static TransactionStatus ToModelStatus(this TransactionStatusXml status)
        {
            return status switch
            {
                TransactionStatusXml.Approved => TransactionStatus.A,
                TransactionStatusXml.Failed => TransactionStatus.R,
                TransactionStatusXml.Finished => TransactionStatus.D,
                _ => throw new InvalidOperationException("Incorrect xml status")
            };
        }
    }
    
    public class TransactionFormatCsv : IValidatableObject
    {
        [StringLength(50, MinimumLength = 1,ErrorMessage = "Transaction id must be at least 1 and not more than 50 characters long")]
        public string TransactionIdentificator { get; set; }
        public decimal Amount { get; set; }
        [StringLength(3, ErrorMessage = "Currency code must be 3 characters long")]
        public string CurrencyCode { get; set; }
        public string TransactionDate { get; set; }
        public string Status { get; set; }

        public Transaction ToTransactionModel()
        {
            return new Transaction()
            {
                TransactionId = this.TransactionIdentificator,
                Amount = this.Amount,
                Currency = this.CurrencyCode,
                TransactionDate = DateTime.ParseExact(this.TransactionDate, "dd/MM/yyyy hh:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None),
                Status = ((TransactionStatusCsv)Enum.Parse(typeof(TransactionStatusCsv), this.Status)).ToModelStatus()
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (!ValidationHelper.IsValidCsvDate(this.TransactionDate))
            {
                results.Add(new ValidationResult("Date must be in correct format."));
            }
            if (!ValidationHelper.IsValidCurrency(this.CurrencyCode))
            {
                results.Add(new ValidationResult("Currency code is not of ISO4217 format"));
            }

            if (!ValidationHelper.IsValidCsvStatus(this.Status))
            {
                results.Add((new ValidationResult("Invalid transaction status")));
            }
            
            return results;

           
        }
    }
}