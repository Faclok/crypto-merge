using BusLogic.Services;
using InternetDatabase.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace crypto_merge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(WalletService walletService) : ControllerBase
{
    [HttpGet("{idCC}")]
    public async Task<IActionResult> GetById(string idCC)
    {
        return Ok(await walletService.GetTransactionByIdCCAsync(idCC));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string idCC)
    {
        var transactions = await walletService.Search(idCC);
       return Ok(transactions.Select(t => t.GetTransactionDTO()));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSum(int id,decimal sum, [FromServices] TransactionService transactionService)
    {
        await transactionService.TryUpdateSumAsync(id, sum, "Пополнение");
        return NoContent();
    } 
}

