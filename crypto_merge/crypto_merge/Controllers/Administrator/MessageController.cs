using BusLogic.Services;
using crypto_merge.Tg.Bot.Services;
using Microsoft.AspNetCore.Mvc;

namespace crypto_merge.Controllers.Administrator
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController(MessageService messageService, MessageSenderFromAdmin telegramSender, MessageSender messageSender) : ControllerBase
    {
        [HttpGet("transactions/{transactionId}")]
        public async Task<IActionResult> GetByTransactionId(int transactionId)
        {
            return Ok(await messageService.GetByTransactionIdAsync(transactionId));
        }

        [HttpPost("transactions/{transactionId}")]
        public async Task<IActionResult> Post(int transactionId, string? tag, [FromBody] RequestMessage requestMessage)
        {
            var id = await messageService.SendFromAdmin(transactionId, requestMessage.Message, tag);
            await telegramSender.SendFromAdmin(transactionId, requestMessage.Message);

            return Ok(id);
        }

        [HttpPost("transactions/{transactionId}/request")]
        public async Task<IActionResult> PostRequest(int transactionId, string? tag, [FromBody] RequestRequestMessage requestMessage)
        {
            var id = await messageService.SendFromAdmin(transactionId, requestMessage.Message, tag);
            await telegramSender.SendFromAdmin(transactionId, requestMessage.TitleRequest, requestMessage.Request, requestMessage.TitleSum, requestMessage.Sum, requestMessage.Comment);

            return Ok(id);
        }

        [HttpPost("chat/{chatId}")]
        public async Task<IActionResult> Post(long chatId, string message)
        {
            await messageSender.SendMessageAsync(chatId, message);

            return NoContent();
        }
    }

    public record RequestMessage(string Message);
    public record RequestRequestMessage(string TitleRequest, string Request, string TitleSum, string Sum, string Comment, string Message);
}
