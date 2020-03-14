using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionApi.Server.Data;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Shared.Dto;

namespace TransactionApi.Server.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly TransactionDbContext _dbContext;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(TransactionDbContext dbContext, ILogger<TransactionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        public async Task ProcessTransactionsAsync(IEnumerable<Transaction> transactions)
        {
            await using var tr = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var trItem in transactions)
                {
                    var trEntity = await _dbContext.Transaction.FindAsync(trItem);
                    if (trEntity != null)
                    {
                        _dbContext.Transaction.Update(trEntity);
                    }
                    else
                    {
                        await _dbContext.Transaction.AddAsync(trItem);
                    }

                    await _dbContext.SaveChangesAsync();
                    await tr.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transactions");
                await tr.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<TransactionDto>> QueryTransactionsAsync(TransactionQuery query)
        {
            try
            {
                var dbSet = _dbContext.Transaction.AsNoTracking();
                if (!string.IsNullOrEmpty(query.CurrencyCode))
                {
                    dbSet = dbSet.Where(tr => tr.Currency == query.CurrencyCode);
                }
                if (query.StartTime.HasValue && query.EndTime.HasValue)
                {
                    dbSet = dbSet.Where(tr => tr.TransactionDate > query.StartTime.Value &&
                                              tr.TransactionDate < query.EndTime.Value);
                }
                if (!string.IsNullOrEmpty(query.Status))
                {
                    dbSet = dbSet.Where(tr => tr.Status == query.Status);
                }

                return await dbSet.Select(tr => new TransactionDto()
                {
                    Id = tr.TransactionId,
                    Payment = $"{tr.Amount} {tr.Currency}",
                    Status = tr.Status
                }).ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error querying transactions");
                throw;
            }
        }
    }
}