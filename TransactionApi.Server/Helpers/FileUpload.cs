using Microsoft.AspNetCore.Http;

namespace TransactionApi.Server.Helpers
{
    public class FileUpload
    {
        public string Name { get; set; }
        public IFormFile Content { get; set; }
    }
}