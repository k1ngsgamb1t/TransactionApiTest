using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransactionApi.Server.Exceptions
{
    public class InvalidFileDataException : Exception
    {
        private IDictionary<string, List<ValidationResult>> _errors;

        public InvalidFileDataException(Dictionary<string, List<ValidationResult>> errors)
        :base("Data is invalid in the uploaded file")
        {
            _errors = errors;
        }
    }
}