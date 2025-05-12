using BusLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace crypto_merge.Controllers.Administrator
{
    [ApiController]
    [Route("api/[controller]")]
    public class LimitsController(WalletService walletService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetLimits([FromServices] CurrencyService currencyService)
        {
            return Ok(await walletService.GetLimitsAsync((await currencyService.GetCurrencyAsync("USD"))!.RubCurrency));
        }


        [HttpGet("count")]
        public async Task<IActionResult> GetLimitsCount()
        {
            return Ok(await walletService.GetLimitsCountAsync());
        }

        [HttpPut("{walletId}")]
        public async Task<IActionResult> PutCheck(int walletId)
        {
            await walletService.PutLimitAsync(walletId);
            return NoContent();
        }

        [HttpGet("transaction/{id}")]
        public async Task<IActionResult> GetLimit(int id)
        {
            return Ok(await walletService.GetLimitYesterdayOnTransactionAsync(id, "USD"));
        }
    }
}
