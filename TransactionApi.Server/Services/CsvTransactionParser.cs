using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Server.Services.Formats;

namespace TransactionApi.Server.Services
{
    public class CsvTransactionParser : ITransactionParser
    {
        public async IAsyncEnumerable<Transaction> Parse(StreamReader sourceString)
        {
            using var csvReader = new CsvReader(sourceString, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                IgnoreBlankLines = true,
                Delimiter = ",",
                TrimOptions = TrimOptions.InsideQuotes | TrimOptions.Trim,
                CountBytes = true,
                Encoding = Encoding.UTF8
            });

            var csvTransaction = new TransactionFormatCsv();
            await foreach (var csvItem in csvReader.EnumerateRecordsAsync<TransactionFormatCsv>(csvTransaction))
            {
                yield return csvItem.ToTransactionModel();
            }
        }
    }
}