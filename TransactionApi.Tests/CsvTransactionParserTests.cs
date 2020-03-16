using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using TransactionApi.Server.Services;
using Xunit;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Shared.Enums;

namespace TransactionApi.Tests
{
    public class CsvTransactionParserTests
    {
        public static class TestData
        {
            public const string File_CorrectCsvData = @"TestData/transactionsCsv.csv";
        }

        private ITransactionParser _csvParser;

        public CsvTransactionParserTests()
        {
            _csvParser = new CsvTransactionParser();
        }

        [Fact]
        public async Task CsvParserTest_AllOk()
        {
            //Arrange
            using var streamReader = new StreamReader(TestData.File_CorrectCsvData);
            
            //Act
            var items = await _csvParser.Parse(streamReader).ToListAsync();
            
            //Assert
            Assert.Equal(2, items.Count());
            Assert.Equal("Invoice0000001", items[0].TransactionId);
            Assert.Equal(1000, items[0].Amount);
            Assert.Equal("USD", items[0].Currency);
            Assert.Equal(TransactionStatus.A, items[0].Status);
            Assert.Equal("20/02/2019 12:33:16", items[0].TransactionDate.ToString("dd/MM/yyyy hh:mm:ss"));
            
            Assert.Equal("Invoice0000002", items[1].TransactionId);
            Assert.Equal(300, items[1].Amount);
            Assert.Equal("USD", items[1].Currency);
            Assert.Equal(TransactionStatus.R, items[1].Status);
            Assert.Equal("21/02/2019 02:04:59", items[1].TransactionDate.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        [Fact]
        public async Task CsvParserTest_IncorrectFormat()
        {
            //Arrange
            var msstream = new MemoryStream();
            var writer = new StreamWriter(msstream);
            writer.Write("some&text&incorrect&");
            await writer.FlushAsync();
            using var stream = new StreamReader(msstream);
            //Act & Assert
            await _csvParser.Parse(stream).ToListAsync();
        }
    }
}