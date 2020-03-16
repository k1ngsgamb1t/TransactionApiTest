using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Server.Validations;

namespace TransactionApi.Server.Services.Formats
{
    public enum TransactionStatusXml
    {
        Approved = 0,
        Rejected = 1,
        Done = 2
    }

    [XmlRoot("Transactions")]
    public class TransactionsList
    {
        [XmlElement("Transaction")]
        public List<TransactionFormatXml> Transactions { get; set; }
    }
    
    [Serializable]
    [XmlRoot("Transaction")]
    public class TransactionFormatXml : ITransactionFormat, IValidatableObject
    {
        public class PaymentDetailsInfo
        {
            [XmlElement]
            public decimal Amount { get; set; }
            
            [XmlElement]
            [StringLength(3, ErrorMessage = "Currency code must be 3 characters long")]
            public string CurrencyCode { get; set; }
        }
        [StringLength(50, MinimumLength = 1,ErrorMessage = "Transaction id must be at least 1 and not more than 50 characters long")]
        [XmlAttribute("id")]
        public string TransactionIdentificator { get; set; }
        [XmlElement("PaymentDetails")]
        public PaymentDetailsInfo PaymentDetails { get; set; }
        [XmlElement]
        public string TransactionDate { get; set; }
        [XmlElement]
        public string Status { get; set; }

        public string GetItemId()
        {
            return string.IsNullOrEmpty(TransactionIdentificator)
                ? $"{PaymentDetails.Amount}:{PaymentDetails.CurrencyCode}:{TransactionDate}:{Status}"
                : TransactionIdentificator;
        }

        public Transaction ToTransactionModel()
        {
            return new Transaction()
            {
                TransactionId = this.TransactionIdentificator,
                Amount = this.PaymentDetails.Amount,
                Currency = this.PaymentDetails.CurrencyCode,
                TransactionDate = DateTime.ParseExact(this.TransactionDate, "yyyy-MM-ddTHH:mm:ss",
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