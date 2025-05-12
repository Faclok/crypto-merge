using BusLogic.Services;
using crypto_merge.Tg.Bot.Services;
using Microsoft.AspNetCore.Mvc;

namespace crypto_merge.Controllers.Administrator
{

    [ApiController]
    [Route("api/[controller]")]
    public class DepositsController(DepositService depositService, WalletService walletService) : ControllerBase
    {

        /// <summary>
        /// Получение всех депозитов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await depositService.GetActiveDepositsAsync(walletService.GetTotalBalanceAndTempAsync, "USD"));
        }

        [HttpGet("accountCryptoCards/{accountCryptoCardId}")]
        public async Task<IActionResult> GetAsync(int accountCryptoCardId)
        {
            return Ok(await depositService.GetActiveDepositsOnCCAsync(walletService.GetTotalBalanceAndTempAsync, "USD", accountCryptoCardId));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> Close(int id, [FromServices] TransactionService transactionService, [FromServices] MessageSenderFromAdmin messageSenderFromService, bool isCompleted = false, bool isClose = false)
        {
            if (isClose)
            {
                var put = await transactionService.GetSumAsync(id);
                await messageSenderFromService.SendFromAdmin(id, $"Заявка на пополнение {put} ОТМЕНЕНА. Перевод средств по этой заявке ВЕДЕТ К ПОТЕРИ ДЕНЕГ!!!");
                await walletService.DeleteTransaction(id);
                return NoContent();
            }

            if (isCompleted)
            {
                await walletService.UpdateTransactionStatus(id,InternetDatabase.EntityDB.TransactionStatus.Completed);
                await transactionService.UpdateDateTimeUTC(id, DateTime.UtcNow);
                await transactionService.CreateReferralDeposit(id, await transactionService.GetSumAsync(id), 0.005m);
                await transactionService.AddPercentOnSumAsync(id, 0.045m);
                var balance = await walletService.GetLimitYesterdayOnTransactionAsync(id, "USD");
                var boostBalance = await walletService.GetBalanceBoostOnTransaction(id, "USD");
                var plus = await transactionService.GetSumAsync(id, "USD");
                await messageSenderFromService.SendTransactionCompleted(id);

                return Ok(new ResponseCopies[]
                {
                    new("Начислено", plus),
                    new("Тык", balance),
                    new("Буст. тык", balance + boostBalance),
                });
            }

            return BadRequest();
        }
    }

    public record ResponseCopies(string Name, decimal Value);
}
