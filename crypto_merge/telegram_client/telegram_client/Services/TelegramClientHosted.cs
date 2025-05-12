using B = BusLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TL;
using WTelegram;
using System.Text.RegularExpressions;
using System.Globalization;

namespace telegram_client.Services;

public class TelegramClientHosted(IServiceProvider serviceProvider, TelegramClient client) : BackgroundService
{
    private UpdateManager? _manager;
    private InputPeer? _inputPeer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var isLogin = await client.LoginAsync();

        if (!isLogin)
        {
            await Console.Out.WriteLineAsync("fail start " + nameof(TelegramClientHosted));
            return;
        }


        await Console.Out.WriteLineAsync("success start " + nameof(TelegramClientHosted));


        try
        {
            var messageChats = await client.Messages_GetAllDialogs();

            _inputPeer = messageChats.users.First(o => o.Value.MainUsername == "cryptocardsclub_bot").Value;
            var chat = await client.Messages_GetHistory(_inputPeer);

            var messages = await client.Messages_GetHistory(_inputPeer, offset_date: DateTime.UtcNow.AddHours(3).Date);

            var tasks = new List<Task>();
            foreach (var msgBase in messages.Messages)
            {
                var from = messages.UserOrChat(msgBase.From ?? msgBase.Peer); // from can be User/Chat/Channel
                if (msgBase is Message msg)
                    tasks.Add(HandleMessage(msg));
                else if (msgBase is MessageService ms)
                    Console.WriteLine($"{from} [{ms.action.GetType().Name[13..]}]");
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception)
        {
        }

        _manager = client.WithUpdateManager(Client_OnUpdate);
    }

    // if not using async/await, we could just return Task.CompletedTask
    private async Task Client_OnUpdate(Update update)
    {
        switch (update)
        {
            case UpdateNewMessage unm: await HandleMessage(unm.message); break;
            // Note: UpdateNewChannelMessage and UpdateEditChannelMessage are also handled by above cases   
            default: Console.WriteLine(update.GetType().Name); break; // there are much more update types than the above example cases
        }
    }

    // in this example method, we're not using async/await, so we just return Task.CompletedTask
    private async Task HandleMessage(MessageBase messageBase)
    {
        switch (messageBase)
        {
            case Message m:
                Console.WriteLine($"{Peer(m.from_id) ?? m.post_author} in {Peer(m.peer_id)}> {m.message}");

                if (_manager?.UserOrChat(m.from_id)?.ToString() == "@cryptocardsclub_bot")
                {
                    var message = m.message;

                    var messageSendText = "Увидел сообщение";

                    if (await ParseRequestAsync(message) is { } messageParse)
                        messageSendText = messageParse;
                    else if(await ParseRequestIceAsync(message) is { } messageParseIce)
                        messageSendText = messageParseIce;

                    await client.SendMessageAsync(_inputPeer, messageSendText);
                }
                break;
            case MessageService ms: Console.WriteLine($"{Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]"); break;
        }
    }

    private async Task<string?> ParseRequestAsync(string message)
    {

        // Регулярное выражение для суммы в рублях
        string rubAmountPattern = @"\((\d+(?:\.\d{1,2})?)₽\)";
        Match rubAmountMatch = Regex.Match(message, rubAmountPattern);
        string rubAmount = rubAmountMatch.Success ? rubAmountMatch.Groups[1].Value : "Не найдено";


        // Регулярное выражение для остатка
        string balancePattern = @"Остаток: \$(\d+\.\d{2})";
        Match balanceMatch = Regex.Match(message, balancePattern);
        string balance = balanceMatch.Success ? balanceMatch.Groups[1].Value : "Не найдено";

        // Регулярное выражение для номера карты
        string cardPattern = @"💳 (\d+)";
        Match cardMatch = Regex.Match(message, cardPattern);
        string? cardNumber = cardMatch.Success ? cardMatch.Groups[1].Value : null;

        // Регулярное выражение для номера телефона
        string phonePattern = @"🏦 (\+?\d+)";
        Match phoneMatch = Regex.Match(message, phonePattern);
        string? phoneNumber = phoneMatch.Success ? phoneMatch.Groups[1].Value : null;

        // Выбор того, что нашли - номер карты или телефон
        string cardOrPhone = cardNumber ?? phoneNumber ?? "Не найдено";

        // Регулярное выражение для информации
        string infoPattern = @"ℹ️ #(\d+) • (\d{2}\.\d{2}\.\d{2} \d{2}:\d{2}:\d{2})";
        Match infoMatch = Regex.Match(message, infoPattern);
        string infoNumber = infoMatch.Success ? infoMatch.Groups[1].Value : "Не найдено";
        string infoDate = infoMatch.Success ? infoMatch.Groups[2].Value : "Не найдено";

        if (cardOrPhone.Contains('+'))
        {
            cardOrPhone = cardOrPhone.Replace("+", string.Empty);
            if (cardOrPhone[0] == '7')
                cardOrPhone = string.Concat("8", cardOrPhone.AsSpan(1));
        }

        var messageSendText = "Увидел заявку: " + infoNumber;

        Console.WriteLine("------------------------------------------");
        //Console.WriteLine($"ID CREATE SYSTEM: {result.Id}");
        Console.WriteLine($"Сумма: {rubAmount}₽");
        Console.WriteLine($"Остаток: ${balance}");
        Console.WriteLine($"Номер карты/телефон: {cardOrPhone}");
        Console.WriteLine($"Номер информации: {infoNumber}");
        Console.WriteLine($"Дата и время: {infoDate}");
        Console.WriteLine(messageSendText);
        Console.WriteLine("------------------------------------------");

        if (decimal.TryParse(rubAmount, out var amount) && decimal.TryParse(cardOrPhone, out var _) && decimal.TryParse(infoNumber, out var _) && DateTime.TryParseExact(infoDate, "dd.MM.yy HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date))
        {
            messageSendText += "\nСмог распознать данные";

            var walletService = serviceProvider.GetRequiredService<B.WalletService>();

            if (!await walletService.IsCheckAsync(infoNumber))
            {
                messageSendText += "\nЗаявка является не дубликатом";
                if (await walletService.CreateTransactionCC(new() { RequestProperty = cardOrPhone, TransactionIdCC = infoNumber, Count = amount }, date) is { } result)
                    return messageSendText + "\nЗаявка зарегистрирована: " + result.Id;
            }
        }

        return null;
    }

    private async Task<string?> ParseRequestIceAsync(string message)
    {
        // Регулярное выражение для номера заявки
        string requestNumberPattern = @"#(\d+)";
        Match requestNumberMatch = Regex.Match(message, requestNumberPattern);
        string requestNumber = requestNumberMatch.Success ? requestNumberMatch.Groups[1].Value : "Не найдено";

        // Регулярное выражение для суммы в долларах
        string dollarAmountPattern = @"в размере \$(\d+\.\d{2})";
        Match dollarAmountMatch = Regex.Match(message, dollarAmountPattern);
        string dollarAmount = dollarAmountMatch.Success ? dollarAmountMatch.Groups[1].Value : "Не найдено";

        // Регулярное выражение для суммы в рублях
        string rubAmountPattern = @"\((\d+(?:\.\d{1,2})?)₽\)";
        Match rubAmountMatch = Regex.Match(message, rubAmountPattern);
        string rubAmount = rubAmountMatch.Success ? rubAmountMatch.Groups[1].Value : "Не найдено";

        // Регулярное выражение для даты и времени
        string datePattern = @"от (\d{2}\.\d{2}\.\d{2} \d{2}:\d{2}:\d{2})";
        Match dateMatch = Regex.Match(message, datePattern);
        string date = dateMatch.Success ? dateMatch.Groups[1].Value : "Не найдено";

        // Регулярное выражение для номера карты или телефона
        string cardPattern = @"💳 (\d+)";
        string phonePattern = @"🏦 (\+?\d+)";
        Match cardMatch = Regex.Match(message, cardPattern);
        Match phoneMatch = Regex.Match(message, phonePattern);
        string cardOrPhone = cardMatch.Success ? cardMatch.Groups[1].Value :
                             phoneMatch.Success ? phoneMatch.Groups[1].Value : "Не найдено";

        // Регулярное выражение для реквизитов (в круглых скобках после карты/телефона)
        string detailsPattern = @"\((\d+)\(\d+\)\)";
        Match detailsMatch = Regex.Match(message, detailsPattern);
        string details = detailsMatch.Success ? detailsMatch.Groups[1].Value : "Не найдено";

        // Регулярное выражение для остатка
        string balancePattern = @"Остаток: \$(\d+\.\d{2})";
        Match balanceMatch = Regex.Match(message, balancePattern);
        string balance = balanceMatch.Success ? balanceMatch.Groups[1].Value : "Не найдено";

        if (cardOrPhone.Contains('+'))
        {
            cardOrPhone = cardOrPhone.Replace("+", string.Empty);
            if (cardOrPhone[0] == '7')
                cardOrPhone = string.Concat("8", cardOrPhone.AsSpan(1));
        }

        // Вывод результатов
        Console.WriteLine($"Номер заявки: {requestNumber}");
        Console.WriteLine($"Сумма в долларах: ${dollarAmount}");
        Console.WriteLine($"Сумма в рублях: {rubAmount}₽");
        Console.WriteLine($"Дата и время: {date}");
        Console.WriteLine($"Номер карты/телефон: {cardOrPhone}");
        Console.WriteLine($"Реквизиты: {details}");
        Console.WriteLine($"Остаток: ${balance}");

        var messageSendText = "Увидел заявку: " + requestNumber;
        if (decimal.TryParse(rubAmount, out var amount) && decimal.TryParse(cardOrPhone, out var _) && decimal.TryParse(requestNumber, out var _) && DateTime.TryParseExact(date, "dd.MM.yy HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateParse))
        {
            messageSendText += "\nСмог распознать данные";

            var walletService = serviceProvider.GetRequiredService<B.WalletService>();

            if (!await walletService.IsCheckAsync(requestNumber))
            {
                messageSendText += "\nЗаявка является не дубликатом";
                if (await walletService.CreateTransactionCC(new() { RequestProperty = cardOrPhone, TransactionIdCC = requestNumber, Count = amount }, dateParse) is { } result)
                    return messageSendText + "\nЗаявка зарегистрирована: " + result.Id;
            }
        }

        return null;
    }

    private string Peer(Peer peer) => _manager?.UserOrChat(peer)?.ToString() ?? $"Peer {peer?.ID}";
}
