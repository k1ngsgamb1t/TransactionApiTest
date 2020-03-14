using System;

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
        public string TransactionIdentificator { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionStatusXml Status { get; set; }
    }
}