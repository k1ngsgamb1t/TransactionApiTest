using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TransactionApi.Server.Services.Formats;

namespace TransactionApi.Server.Validations
{
    public static class ValidationHelper
    {
        public static async Task<bool> TryValidateObjectsWithKey<T>(IAsyncEnumerable<T> objects,
            Func<T, string> keyAccessor, Dictionary<string, List<ValidationResult>> validationMap)
        {
            var isValid = true;
            await foreach (var item in objects)
            {
                var vc = new ValidationContext(item);
                var validationResults = new List<ValidationResult>();
                isValid &= Validator.TryValidateObject(item, vc, validationResults);
                if (!isValid)
                    validationMap[keyAccessor(item)] = validationResults;
            }

            return isValid;
        }
        
        public static bool IsValidXmlDate(string dateString)
        {
            return DateTime.TryParseExact(dateString, "yyyy-MM-ddThh:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _);
        }

        public static bool IsValidCurrency(string currencyCode)
        {
            var currencySymbols = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures) //Only specific cultures contain region information
                .Select(x => (new RegionInfo(x.LCID)).ISOCurrencySymbol)
                .Distinct()
                .OrderBy(x => x);
            return currencySymbols.Contains(currencyCode);
        }

        public static bool IsValidXmlStatus(string status)
        {
            return Enum.TryParse(typeof(TransactionStatusXml), status, ignoreCase: false, out _);
        }
        
        public static bool IsValidCsvDate(string dateString)
        {
            return DateTime.TryParseExact(dateString, "dd/MM/yyyy hh:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _);
        }

        public static bool IsValidCsvStatus(string status)
        {
            return Enum.TryParse(typeof(TransactionStatusCsv), status, ignoreCase: false, out _);
        }
    }
}