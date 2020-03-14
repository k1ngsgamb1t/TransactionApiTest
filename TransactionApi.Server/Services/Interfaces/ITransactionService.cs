using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionApi.Server.Data.Entities;
using TransactionApi.Shared.Dto;

namespace TransactionApi.Server.Services.Interfaces
{
    public interface ITransactionService
    {
        Task ProcessTransactionsAsync(IEnumerable<Transaction> transactions);
        Task<IEnumerable<TransactionDto>> QueryTransactionsAsync(TransactionQuery query);
    }
}