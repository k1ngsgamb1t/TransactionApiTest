using System;

namespace TransactionApi.Server.Data.Entities
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
    }
}