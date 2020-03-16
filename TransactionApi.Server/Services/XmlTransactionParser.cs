using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Server.Services.Formats;

namespace TransactionApi.Server.Services
{
    public class XmlTransactionParser : ITransactionParser
    {
        public string SupportedExtension { get; } = "xml";

        public async IAsyncEnumerable<Transaction> Parse(StreamReader sourceString)
        {
            var xmlSerializer = new XmlSerializer(typeof(TransactionsList));
            var list = (TransactionsList) xmlSerializer.Deserialize(sourceString);
            foreach (var xmlItem in list.Transactions)
            {
                yield return await Task.FromResult(xmlItem.ToTransactionModel());
            }
        }
    }
}