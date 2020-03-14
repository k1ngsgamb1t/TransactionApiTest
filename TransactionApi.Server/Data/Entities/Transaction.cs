using System;
using TransactionApi.Shared.Dto;
using TransactionApi.Shared.Enums;

namespace TransactionApi.Server.Data.Entities
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionStatus Status { get; set; }

        public TransactionDto ToDto()
        {
            return new TransactionDto()
            {
                Id = this.TransactionId,
                Payment = $"{this.Amount} {this.Currency}",
                Status = this.Status
            };
        }
    }
}