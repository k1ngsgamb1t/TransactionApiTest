using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Services.Interfaces;

namespace TransactionApi.Server.Services
{
    public class XmlTransactionParser : ITransactionParser
    {
        public async IAsyncEnumerable<Transaction> Parse(StreamReader sourceString)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<TransactionFormatXml>), new XmlRootAttribute("Transactions"));
            foreach (var xmlItem in (List<TransactionFormatXml>)xmlSerializer.Deserialize(sourceString))
            {
                yield return await Task.FromResult(xmlItem.ToTransactionModel());
            }
        }
    }
}