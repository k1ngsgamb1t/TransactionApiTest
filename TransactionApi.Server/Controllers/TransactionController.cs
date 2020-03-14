using TransactionApi.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TransactionApi.Server.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Produces("application/json")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> logger;

        public TransactionController(ILogger<TransactionController> logger)
        {
            this.logger = logger;
        }
        
        [HttpPost]
        [Route("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadTransactionsAsync([FromForm]IFormFile transactionsSource)
        {
             return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsAsync([FromQuery]TransactionQuery query)
        {
             var dummyResp = new List<TransactionDto>(); 
             return Ok(dummyResp);
        }
    }
}
