using BusLogic.Services;
using InternetDatabase.EntityDB;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace crypto_merge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestTelegramController(ITelegramBotClient botClient, WalletService walletService) : ControllerBase
    {

        [HttpGet("me")]
        public async Task<IActionResult> GetName()
        {
            var botName = await botClient.GetMeAsync();

            return Ok(botName);
        }

        [HttpGet("commands")]
        public async Task<IActionResult> GetCommands()
        {
            var botCommands = await botClient.GetMyCommandsAsync();

            return Ok(botCommands);
        }

        [HttpPost("wallet")]
        public async Task<IActionResult> WalletAdd(int walletId, int count, string message, TransactionType type, TransactionStatus status)
        {
            await walletService.AddMoneyTransactionAsync(walletId, 1, count, message, type, status);
            return Ok(await walletService.GetTransactionWalletAsync(walletId));
        }
    }
}
