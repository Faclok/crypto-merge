using BusLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace crypto_merge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountCryptoCardsController(AccountCryptoCardService cryptoCardService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok((await cryptoCardService.GetAllAsync()).Select(o => new{ o.Id, o.Name }).ToArray());

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            await cryptoCardService.CreateAsync(name);

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutName(int id, string name)
        {
            if(!await cryptoCardService.PutNameAsync(id, name))
                return NotFound();


            return NoContent();
        }
    }
}
