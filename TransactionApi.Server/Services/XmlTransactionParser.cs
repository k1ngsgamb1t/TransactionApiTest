using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Exceptions;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Server.Services.Formats;

namespace TransactionApi.Server.Services
{
    public class XmlTransactionParser : ITransactionParser
    {
        public string SupportedExtension { get; } = "xml";
        private readonly Dictionary<string, List<ValidationResult>> _validationMap =
        new Dictionary<string, List<ValidationResult>>();

        public async IAsyncEnumerable<Transaction> Parse(StreamReader sourceString)
        {
            var xmlSerializer = new XmlSerializer(typeof(TransactionsList));
            var list = (TransactionsList) xmlSerializer.Deserialize(sourceString);
            foreach (var xmlItem in list.Transactions)
            {
                var validationResults = new List<ValidationResult>();
                if(Validator.TryValidateObject(xmlItem, new ValidationContext(xmlItem), validationResults))
                    yield return await Task.FromResult(xmlItem.ToTransactionModel());
                else
                {
                    _validationMap[$"record [{xmlItem.TransactionIdentificator}, {xmlItem.Status}, {xmlItem.TransactionDate}], {xmlItem.PaymentDetails.Amount}, {xmlItem.PaymentDetails.CurrencyCode}"] = validationResults;
                }
                if(_validationMap.Keys.Count > 0)
                    throw new InvalidFileDataException(_validationMap);
            }
        }
    }
}