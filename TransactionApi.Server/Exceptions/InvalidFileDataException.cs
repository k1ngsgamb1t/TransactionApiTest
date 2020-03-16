using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TransactionApi.Server.Exceptions
{
    public class InvalidFileDataException : Exception
    {
        private readonly IDictionary<string, List<ValidationResult>> _errors;

        public InvalidFileDataException(Dictionary<string, List<ValidationResult>> errors)
        :base("Data is invalid in the uploaded file")
        {
            _errors = errors;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(_errors);
        }
    }
}