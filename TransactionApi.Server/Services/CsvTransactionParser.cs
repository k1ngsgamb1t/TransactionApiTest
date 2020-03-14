using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Newtonsoft.Json;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Services.Interfaces;

namespace TransactionApi.Server.Services
{
    public class CsvTransactionParser : ITransactionParser
    {
        public async IAsyncEnumerable<Transaction> Parse(StreamReader sourceString)
        {
            using var csvReader = new CsvReader(sourceString, CultureInfo.InvariantCulture);
            var csvTransaction = new TransactionFormatCsv();
            await foreach (var csvItem in csvReader.EnumerateRecordsAsync<TransactionFormatCsv>(csvTransaction))
            {
                yield return csvItem.ToTransactionModel();
            }
        }
    }
}