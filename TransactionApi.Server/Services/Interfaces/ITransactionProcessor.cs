using System.Collections.Generic;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Helpers;

namespace TransactionApi.Server.Services.Interfaces
{
    public interface ITransactionProcessor
    {
        IAsyncEnumerable<Transaction> ProcessFile(FileUpload fUpload);
    }
}