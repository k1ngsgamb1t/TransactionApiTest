using TransactionApi.Shared.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TransactionApi.Server.Helpers;
using TransactionApi.Server.Services.Interfaces;
using TransactionApi.Server.Validations;

namespace TransactionApi.Server.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Produces("application/json")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionProcessor _transactionProcessor;
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService,
            ITransactionProcessor transactionProcessor,
            ILogger<TransactionController> logger)
        {
            _transactionProcessor = transactionProcessor;
            _transactionService = transactionService;
            this._logger = logger;
        }
        
        [HttpPost]
        [DisableRequestSizeLimit]
        [Route("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadTransactionsAsync([FromForm]FileUpload transactionsSource)
        {
            try
            {
                var transactionItems =  _transactionProcessor.ProcessFile(transactionsSource);
                await _transactionService.ProcessTransactionsAsync(transactionItems);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsAsync([FromQuery]TransactionQuery query)
        {
             var resp = await _transactionService.QueryTransactionsAsync(query); 
             return Ok(resp);
        }
    }
}
