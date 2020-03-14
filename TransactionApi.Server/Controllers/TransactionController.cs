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
        private readonly ITransactionParser _csvParser;
        private readonly ITransactionParser _xmlParser;
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService,
            IEnumerable<ITransactionParser> parsers,
            ILogger<TransactionController> logger)
        {
            _csvParser = parsers.First();
            _xmlParser = parsers.ElementAt(1);
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
                using (var reader = new StreamReader(transactionsSource.Content.OpenReadStream()))
                {
                    if (transactionsSource.Name.EndsWith(".csv"))
                    {
                        var transactionItems = _csvParser.Parse(reader);
                        var validationMap = new Dictionary<string, List<ValidationResult>>();
                        if(await ValidationHelper.TryValidateObjectsWithKey(transactionItems,
                            (xmlItem) => xmlItem.TransactionId,
                            validationMap))
                            await _transactionService.ProcessTransactionsAsync(transactionItems);
                        else
                        {
                            return BadRequest(validationMap);
                        }
                    }
                    else
                    {
                        var transactionItems = _xmlParser.Parse(reader);
                        var validationMap = new Dictionary<string, List<ValidationResult>>();
                        if(await ValidationHelper.TryValidateObjectsWithKey(transactionItems,
                            (xmlItem) => xmlItem.TransactionId,
                            validationMap))
                            await _transactionService.ProcessTransactionsAsync(transactionItems);
                        else
                        {
                            return BadRequest(validationMap);
                        }
                    }
                }
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
