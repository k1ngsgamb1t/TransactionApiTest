using System;
using Microsoft.EntityFrameworkCore;
using TransactionApi.Server.Data;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Shared.Enums;

namespace TransactionApi.Tests
{
    public class TransactionServiceTests
    {
        private readonly TransactionDbContext _dbContext;

        public TransactionServiceTests()
        {
            var builder = new DbContextOptionsBuilder<TransactionDbContext>();
            builder.UseInMemoryDatabase("test_db");
           
            _dbContext = new TransactionDbContext(builder.Options);
            PopulateDatabase();
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
    }
}