using System.Collections.Generic;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Services.Interfaces;

namespace TransactionApi.Server.Services
{
    public class CsvTransactionParser : ITransactionParser
    {
        public IEnumerable<Transaction> Parse(string sourceString)
        {
            throw new System.NotImplementedException();
        }
    }
}