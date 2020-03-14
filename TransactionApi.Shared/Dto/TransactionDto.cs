
using TransactionApi.Shared.Enums;

namespace TransactionApi.Shared.Dto
{
    public class TransactionDto
    {
        public string Id { get; set; }
        public string Payment { get; set; }
        public TransactionStatus Status { get; set; }
    }
}