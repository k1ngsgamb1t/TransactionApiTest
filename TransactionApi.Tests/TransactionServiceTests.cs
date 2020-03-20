using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using TransactionApi.Server.Data;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Services;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Shared.Enums;
using Xunit;

namespace TransactionApi.Tests
{
    public class TransactionServiceTests
    {
        private readonly TransactionDbContext _dbContext;
        private readonly ITransactionService _transactionService;

        public TransactionServiceTests()
        {
            var builder = new DbContextOptionsBuilder<TransactionDbContext>();
            builder.UseInMemoryDatabase("test_db");
           
            _dbContext = new TransactionDbContext(builder.Options);
            PopulateDatabase();
            
            _transactionService = new TransactionService(_dbContext,
                NullLoggerFactory.Instance.CreateLogger<TransactionService>());
        }

        private void PopulateDatabase()
        {
            _dbContext.Transaction.Add(new Transaction()
            {
                TransactionId = "1",
                Amount = 10,
                Currency = "USD",
                TransactionDate = DateTime.UtcNow,
                Status = TransactionStatus.A
            });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task ProcessNewTransaction_SuccesfullyAdded()
        {
            //ARRANGE
            var testTransaction = new Transaction()
            {
                TransactionId = "newId",
                Amount = 1,
                Currency = "EUR",
                TransactionDate = DateTime.Now,
                Status = TransactionStatus.A
            };
            //ACT
            await _transactionService.ProcessTransactionsAsync(new[]{testTransaction}.ToAsyncEnumerable());
            
            //ASSERT
            Assert.True(await _dbContext.Transaction.AsQueryable().AnyAsync(x => x.TransactionId == testTransaction.TransactionId));
        }

        [Fact]
        public async Task ProcessExistingTransaction_SuccesfullyUpdated()
        {
            var testTransaction = new Transaction()
            {
                TransactionId = "1",
                Amount = 15,
                Currency = "EUR",
                TransactionDate = DateTime.Now,
                Status = TransactionStatus.R
            };
            //ACT
            await _transactionService.ProcessTransactionsAsync(new[]{testTransaction}.ToAsyncEnumerable());
            
            //ASSERT
            var updatedTransaction = await _dbContext.Transaction.FindAsync(testTransaction.TransactionId);
            Assert.Equal(testTransaction.Amount, updatedTransaction.Amount);
            Assert.Equal(testTransaction.Currency, updatedTransaction.Currency);
            Assert.Equal(testTransaction.Status, updatedTransaction.Status);
            Assert.Equal(testTransaction.TransactionDate, updatedTransaction.TransactionDate);

        }
    }
}