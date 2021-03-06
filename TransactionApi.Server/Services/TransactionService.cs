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
using TransactionApi.Shared.Enums;

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
        
        public async Task ProcessTransactionsAsync(IAsyncEnumerable<Transaction> transactions)
        {
            try
            {
                await foreach (var trItem in transactions)
                {
                    var trEntity = await _dbContext.Transaction.FindAsync(trItem.TransactionId);
                    if (trEntity != null)
                    {
                        trEntity.Amount = trItem.Amount;
                        trEntity.Currency = trItem.Currency;
                        trEntity.Status = trItem.Status;
                        trEntity.TransactionDate = trItem.TransactionDate;
                        _dbContext.Transaction.Update(trEntity);
                    }
                    else
                    {
                        await _dbContext.Transaction.AddAsync(trItem);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transactions");
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
                    var dbStatus = (TransactionStatus) Enum.Parse(typeof(TransactionStatus),query.Status);
                    dbSet = dbSet.Where(tr => tr.Status == dbStatus);
                }

                return await dbSet.Select(tr => tr.ToDto()).ToListAsync();
            }
            catch(Exception ex)
            {
                //even we have middleware here try-catch might also be useful to log message with more context meaning
                _logger.LogError(ex, "Error querying transactions");
                throw;
            }
        }
    }
}