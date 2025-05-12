using BusLogic.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace crypto_merge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramClientController : ControllerBase
    {

        [HttpPost("code/1/{code}")]
        public async Task<IActionResult> PostCode(int code, [FromServices] TelegramClientLogin login)
        {
            await login.SendCode(code);
            return NoContent();
        }

        [HttpGet("status/1")]
        public IActionResult GetStatus([FromServices] TelegramClientLogin login)
        {
            return Ok(login.IsLogin);
        }

        [HttpPost("code/2/{code}")]
        public async Task<IActionResult> PostCode2(int code, [FromServices] TelegramClientLoginTwo login)
        {
            await login.SendCode(code);
            return NoContent();
        }

        [HttpGet("status/2")]
        public IActionResult GetStatus2([FromServices] TelegramClientLoginTwo login)
        {
            return Ok(login.IsLogin);
        }
    }
}
