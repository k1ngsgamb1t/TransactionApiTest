using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Exceptions;
using TransactionApi.Server.Helpers;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Server.Validations;

namespace TransactionApi.Server.Services
{
    public class TransactionProcessor : ITransactionProcessor
    {
        private readonly IDictionary<string, ITransactionParser> _registeredParsers;

        public TransactionProcessor(IEnumerable<ITransactionParser> parsers)
        {
            _registeredParsers = parsers.ToDictionary(p => p.SupportedExtension);
        }

        private string getExtension(string fileName) => Path.GetExtension(fileName).TrimStart('.');

        public IAsyncEnumerable<Transaction> ProcessFile(FileUpload fUpload)
        {
            var contentExtension = getExtension(fUpload.Content.FileName);
            if (_registeredParsers.ContainsKey(contentExtension))
            {
                using StreamReader stream = new StreamReader(fUpload.Content.OpenReadStream()); 
                return _registeredParsers[contentExtension].Parse(stream);
            }
            else
            {
                throw new FormatNotSupportedException(contentExtension);
            }
        }
    }
}