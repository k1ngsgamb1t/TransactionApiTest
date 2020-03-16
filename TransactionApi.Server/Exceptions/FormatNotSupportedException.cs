using System;

namespace TransactionApi.Server.Exceptions
{
    public class FormatNotSupportedException : Exception
    {
        public FormatNotSupportedException(string formatExtension)
            : base($"{formatExtension} file format is not supported by Api")
        {
            
        }
    }
}