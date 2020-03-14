using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TransactionApi.Server.Services
{
    public class FileTransactionSource
    {
        private readonly IFormFile _sourceFile;

        public FileTransactionSource(IFormFile file)
        {
            _sourceFile = file;
        }

        public async Task ReadSourceToStream(MemoryStream ms)
        {
            await _sourceFile.CopyToAsync(ms);
        }
    }
}