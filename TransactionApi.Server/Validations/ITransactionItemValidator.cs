using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TransactionApi.Server.Services.Interfaces;

namespace TransactionApi.Server.Validations
{
    public interface ITransactionItemValidator
    {
        bool TryValidateItem(ITransactionFormat item);
        void ThrowIfValidationEverFailed();
    }
}