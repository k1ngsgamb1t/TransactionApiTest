using TransactionApi.Server.Data.Entities;

namespace TransactionApi.Server.Services.Interfaces
{
    public interface ITransactionFormat
    {
        string GetItemId();
        Transaction ToTransactionModel();
    }
}