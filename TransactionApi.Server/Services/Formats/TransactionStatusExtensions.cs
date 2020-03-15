using System;
using TransactionApi.Shared.Enums;

namespace TransactionApi.Server.Services.Formats
{
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
}