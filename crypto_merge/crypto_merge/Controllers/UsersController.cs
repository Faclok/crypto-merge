using BusLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace crypto_merge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(WalletService walletService, UserService userService) :ControllerBase
{

    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetByChatId(long chatId)
    {
        var user = await userService.GetByChatIdAsync(chatId);

        if (user is null)
            return NotFound();

        var statistics = new ResponseStatistics()
        {
            ChatId = user.ChatId,
            Username = user.Username,
            Balance = await walletService.GetTotalBalanceAndTempAsync(chatId),
            IncomeRefUsers = await walletService.GetSumMoneyInReferralAndTempAsync(chatId),
            StatisticsAll = await walletService.GetTotalDepositSumAsync(chatId),
            Statistics3Days = await walletService.GetTotalDepositSumAsync(chatId, DateTime.UtcNow.AddDays(-3)),
            NoteAdmin = user.NoteAdmin,
            CountWallets = await userService.GetCountWallets(chatId, true),
        };

        return Ok(statistics);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByRequisites(string requisites)
    {
        var chatId = await userService.SearchByRequisitesAsync(requisites, true);
        return await GetByChatId(chatId);
    }

    [HttpPut("{chatId}/note/{note}")]
    public async Task<IActionResult> PutNote(long chatId, string note)
    {
        if (!await userService.TryUpdateNoteAsync(chatId, note))
            return NotFound();

        return NoContent();
    }


    [HttpPost("{chatId}/newBalance/{balance}")]
    public async Task<IActionResult> PostNewBalance(long chatId, decimal balance, string comment)
    {
        if (!await walletService.TrySetBalanceAsync(chatId, balance, comment))
            return NotFound();

        return NoContent();
    }

    [HttpPost("{chatId}/upBalance/{balance}")]
    public async Task<IActionResult> PostUpBalance(long chatId, decimal balance, string comment)
    {
        if (!await walletService.TryUpBalanceAsync(chatId, balance, comment))
            return NotFound();

        return NoContent();
    }
}

public class ResponseStatistics
{ 
    public long ChatId { get; set; }
    public string? Username { get; set; }
    public decimal Balance { get; set; }
    public decimal StatisticsAll { get; set; }
    public decimal Statistics3Days { get; set; }
    public int CountWallets { get; set; }
    public decimal IncomeRefUsers { get; set; }
    public string? NoteAdmin { get; set; }
}
