using System;
using TransactionApi.Server.Data.Entities;
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
    
    public class TransactionFormatCsv
    {
        public string TransactionIdentificator { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionStatusCsv Status { get; set; }

        public Transaction ToTransactionModel()
        {
            return new Transaction()
            {
                TransactionId = this.TransactionIdentificator,
                Amount = this.Amount,
                Currency = this.CurrencyCode,
                TransactionDate = this.TransactionDate,
                Status = this.Status.ToModelStatus()
            };
        }
    }
}