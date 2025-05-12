using crypto_merge.Tg.Bot.Handlers;
using crypto_merge.Tg.Bot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TG = crypto_merge.Tg.Bot;

namespace crypto_merge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController(TG.Configuration.SecretConfig secret) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot, [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
        {
            if (secret.Configuration.SecretToken is not null && Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != secret.Configuration.SecretToken)
                return Forbid();

            try
            {
                await handleUpdateService.HandleUpdateAsync(bot, update, ct);
            }
            catch (Exception exception)
            {
                await handleUpdateService.HandleErrorAsync(bot, exception, HandleErrorSource.HandleUpdateError, ct);
            }
            return Ok();
        }

        [HttpPost("{chatId}")]
        public async Task<IActionResult> SendMessage(long chatId, [FromServices] MessageSender sender, [FromBody] string text)
        {
            await sender.SendMessageAsync(chatId, text);
            return Ok();
        }
    }
}
