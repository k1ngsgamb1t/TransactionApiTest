using System;
using System.Text;

namespace TransactionApi.Shared.Dto
{
    public class TransactionQuery
    {
        public string CurrencyCode { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }

        public string ToQueryString()
        {
            var builder = new StringBuilder();
            builder.Append("?");
            if (!string.IsNullOrEmpty(CurrencyCode))
            {
                builder.Append($"currencyCode={CurrencyCode}");
            }

            if (StartTime.HasValue && EndTime.HasValue)
            {
                builder.Append($"&startTime={StartTime}&endTime={EndTime}");
            }

            if (!string.IsNullOrEmpty(Status))
            {
                builder.Append($"&status={Status}");
            }

            return builder.ToString();
        }
    }
}