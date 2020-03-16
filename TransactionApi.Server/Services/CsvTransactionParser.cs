using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Exceptions;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Server.Services.Formats;
using TransactionApi.Server.Validations;

namespace TransactionApi.Server.Services
{
    public class CsvTransactionParser : ITransactionParser
    {
        public string SupportedExtension { get; } = "csv";
        private readonly Dictionary<string, List<ValidationResult>> _validationMap =
            new Dictionary<string, List<ValidationResult>>();

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
            
            var lineNum = 1;
            while (await csvReader.ReadAsync())
            {
                var csvItem = csvReader.GetRecord<TransactionFormatCsv>();
                var validationResults = new List<ValidationResult>();
                if(Validator.TryValidateObject(csvItem, new ValidationContext(csvItem), validationResults))
                    yield return csvItem.ToTransactionModel();
                else
                {
                    _validationMap[$"line {lineNum}"] = validationResults;
                }
                lineNum++;
            }
            
            if(_validationMap.Keys.Count > 0)
                throw new InvalidFileDataException(_validationMap);
        }
    }
}