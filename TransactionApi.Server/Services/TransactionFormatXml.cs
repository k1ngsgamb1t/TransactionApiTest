using System;
using TransactionApi.Server.Data.Entities;

namespace TransactionApi.Server.Services
{
    public enum TransactionStatusXml
    {
        Approved = 0,
        Failed = 1,
        Finished = 2
    }
    
    public class TransactionFormatXml
    {
        public class PaymentDetailsInfo
        {
            public decimal Amount { get; set; }
            public string CurrencyCode { get; set; }
        }
        
        public string TransactionIdentificator { get; set; }
        public PaymentDetailsInfo PaymentDetails { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionStatusXml Status { get; set; }
        
        public Transaction ToTransactionModel()
        {
            return new Transaction()
            {
                TransactionId = this.TransactionIdentificator,
                Amount = this.PaymentDetails.Amount,
                Currency = this.PaymentDetails.CurrencyCode,
                TransactionDate = this.TransactionDate,
                Status = this.Status.ToModelStatus()
            };
        }
    }
}