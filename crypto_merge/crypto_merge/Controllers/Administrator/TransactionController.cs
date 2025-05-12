using BusLogic.Requests;
using BusLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace crypto_merge.Controllers.Administrator
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController(WalletService walletService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestTransaction transaction)
        {
            if (await walletService.IsCheckAsync(transaction.TransactionIdCC))
                return Conflict();

            if (await walletService.CreateTransactionCC(transaction) is not { } result)
                return NotFound();

            return Ok(result);
        }


        [HttpPost("search")]
        public async Task<IActionResult> GetWhere([FromBody] RequestSearch transaction)
        {

            if (await walletService.SearchTransactions(transaction.Request, transaction.Cc, transaction.ChatId, transaction.Sum) is not { } result)
                return NotFound();

            return Ok(result);
        }


        [HttpPut("{id}/{sum}")]
        public async Task<IActionResult> PutSum(int id, decimal sum, string comment, [FromServices] TransactionService transactionService)
        {
            if (!await transactionService.TryUpdateSumAsync(id, sum, comment))
                return NotFound();

            return NoContent();
        }

        public record RequestSearch(string? Request, string? Cc, long? ChatId, decimal? Sum);
    }
}
