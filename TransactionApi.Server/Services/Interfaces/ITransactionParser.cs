
using System.Collections.Generic;
using System.IO;
using TransactionApi.Server.Data.Entities;

namespace TransactionApi.Server.Services.Interfaces
{
    public interface ITransactionParser
    {
        string SupportedExtension { get; }
        IAsyncEnumerable<Transaction> Parse(StreamReader sourceString);
    }
}