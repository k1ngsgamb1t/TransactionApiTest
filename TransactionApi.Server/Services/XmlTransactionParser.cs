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
using TransactionApi.Server.Validations;

namespace TransactionApi.Server.Services
{
    public class XmlTransactionParser : ITransactionParser
    {
        public string SupportedExtension { get; } = "xml";

        private readonly ITransactionItemValidator _validator;

        public XmlTransactionParser(ITransactionItemValidator validator)
        {
            _validator = validator;
        }

        public async IAsyncEnumerable<Transaction> Parse(StreamReader sourceString)
        {
            var xmlSerializer = new XmlSerializer(typeof(TransactionsList));
            var list = (TransactionsList) xmlSerializer.Deserialize(sourceString);
            foreach (var xmlItem in list.Transactions)
            {
                if (_validator.TryValidateItem(xmlItem))
                    yield return await Task.FromResult(xmlItem.ToTransactionModel());
            }
            _validator.ThrowIfValidationEverFailed();
        }
    }
}