using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TransactionApi.Server.Services;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Shared.Enums;
using Xunit;

namespace TransactionApi.Tests
{
    public class XmlTransactionParserTests
    {
        private static class TestData
        {
            public const string File_CorrectXmlData = @"TestData/transactionsXml.xml";
        }

        private readonly ITransactionParser _xmlParser;

        public XmlTransactionParserTests()
        {
            _xmlParser = new XmlTransactionParser();
        }

        [Fact]
        public async Task XmlParserTest_AllOk()
        {
            //Arrange
            using var streamReader = new StreamReader(TestData.File_CorrectXmlData);
            
            //Act
            var items = await _xmlParser.Parse(streamReader).ToListAsync();
            
            //Assert
            Assert.Equal(2, items.Count());
            Assert.Equal("Inv00001", items[0].TransactionId);
            Assert.Equal(200, items[0].Amount);
            Assert.Equal("USD", items[0].Currency);
            Assert.Equal(TransactionStatus.D, items[0].Status);
            Assert.Equal("2019-01-23T13:45:10", items[0].TransactionDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            
            Assert.Equal("Inv00002", items[1].TransactionId);
            Assert.Equal(10000, items[1].Amount);
            Assert.Equal("EUR", items[1].Currency);
            Assert.Equal(TransactionStatus.R, items[1].Status);
            Assert.Equal("2019-01-24T16:09:15", items[1].TransactionDate.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }
}