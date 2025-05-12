using BusLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace crypto_merge.Controllers.Administrator
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController(CurrencyService currencyService) : ControllerBase
    {

        [HttpGet("{title}")]
        public async Task<IActionResult> Get(string title)
        {
            if (await currencyService.GetCurrencyAsync(title) is not { } currency)
                return NotFound();

            return Ok(currency);
        }

        [HttpPut("{title}")]
        public async Task<IActionResult> Put(string title, decimal rub)
        {
            var count = await currencyService.PutCurrencyOnRubAsync(title, rub);

            return count > 0 ? NoContent() : NotFound();
        }
    }
}
