using System.Collections.Generic;
using TransactionApi.Server.Data.Entities;

namespace TransactionApi.Server.Services.Interfaces
{
    public interface ITransactionParser
    {
        IEnumerable<Transaction> Parse(string sourceString);
    }
}