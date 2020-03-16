using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TransactionApi.Server.Exceptions;
using TransactionApi.Server.Services.Interfaces;

namespace TransactionApi.Server.Validations
{
    public class TransactionItemValidator : ITransactionItemValidator
    {
        private readonly Dictionary<string, List<ValidationResult>> _validationMap =
            new Dictionary<string, List<ValidationResult>>();
        
        public bool TryValidateItem(ITransactionFormat item)
        {
            var validationResults = new List<ValidationResult>();
            if (Validator.TryValidateObject(item, new ValidationContext(item), validationResults))
                return true;
            _validationMap[item.GetItemId()] = validationResults;
            return false;
        }

        public void ThrowIfValidationEverFailed()
        {
            if(_validationMap.Keys.Count > 0)
                throw new InvalidFileDataException(_validationMap);
        }
    }
}