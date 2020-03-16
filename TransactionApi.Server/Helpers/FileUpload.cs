using Microsoft.AspNetCore.Http;

namespace TransactionApi.Server.Helpers
{
    public class FileUpload
    {
        public IFormFile Content { get; set; }
    }
}