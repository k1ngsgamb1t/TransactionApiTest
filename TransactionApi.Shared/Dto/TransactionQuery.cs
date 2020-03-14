using System;

namespace TransactionApi.Shared.Dto
{
    public class TransactionQuery
    {
        public string CurrencyCode { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
    }
}