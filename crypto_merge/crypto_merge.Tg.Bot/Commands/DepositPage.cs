using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands.Abstractions;
using crypto_merge.Tg.Bot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace crypto_merge.Tg.Bot.Commands
{
    public class DepositPage(MessageSender sender, WaitForUserResponse waitForUserResponse, WalletService walletService) : ITextCommand, ICallbackCommand
    {
        public string[] MessageKeys => ["Пополнение баланса"];

        public string CallbackKey => nameof(DepositPage);

        private readonly List<string> _bank = new List<string>()
        {
            "СБЕР",
            "АЛЬФА",
            "Т-банк",
            "Все банки",
            "СБП",
        };

        public async Task Handler(Message message)
        {
            if(await walletService.GetMyConnectedWallets(message.Chat.Id) <= 0)
            {
                await sender.SendMessageAsync(message.Chat.Id, "У вас нет ни одного активного банка");
                return;
            }

            var sendMessage = new SendMessage()
            {
                Text = "🏦 Выберите банк (можно несколько), на который хотите перевести\r\n\r\n<i>Учитывайте комиссию при переводе с карты на карту или через СБП</i>",
                ParseMode= Telegram.Bot.Types.Enums.ParseMode.Html,
                KeyboardMarkup = Keyboard.
                    CreateInline(_bank
                    .Select(b => new InlineButton("❌" + b, nameof(DepositPage), b.ToLower(), "off"))
                    .ToArray()),
            };
            await sender.SendMessageAsync(message.Chat.Id, sendMessage);
        }

        public async Task Handler(CallbackQuery callbackQuery, string[] args)
        {
            var message = callbackQuery.Message!;

            if (await walletService.GetMyConnectedWallets(message.Chat.Id) <= 0)
            {
                await sender.SendMessageAsync(message.Chat.Id, "У вас нет ни одного активного банка");
                return;
            }

            if (args[0] == "con")
            {
                var sendMessage = new SendMessage()
                {
                    ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html,
                    Text = """
                    <b>❗️ВНИМАНИЕ,</b>  укажите максимальную сумму депозита.
                    Сумма для оплаты может быть ниже, чем вы указали
                    """,
                };

                await sender.EditMessageAsync(
                   chatId: message.Chat.Id,
                   messageId: message.MessageId,
                   sendMessage: sendMessage);
               
               

                while (true)
                {
                    var newMessage = await waitForUserResponse.GetMessageAsync(message.Chat.Id);

                    if (newMessage is null)
                    {
                        await sender
                            .EditMessageAsync(
                            chatId: message.Chat.Id,
                            messageId: message.MessageId,
                            text: "Пополнение отменено");
                        return;
                    }

                    if (decimal.TryParse(newMessage.Text, out var result))
                    {
                        if (result < 1){
                            await sender
                            .EditMessageAsync(
                                chatId: message.Chat.Id,
                                messageId: message.MessageId,
                                text: "Сумма не может быть отрицательной или равна нулю");
                            continue;
                        }

                        await walletService.CreateTransaction(message.Chat.Id, result, args.Skip(1).ToArray());
                        await sender.SendMessageAsync(message.Chat,"""
                            📌Мы приняли вашу заявку 
                            
                            ⌛️Ожидайте, сейчас Вы получите реквизиты и сумму для оплаты. 
                            У Вас будет <b>25 МИНУТ</b> на оплату.
                            ‼️ОБЯЗАТЕЛЬНО переводите точную сумму с банка, который авторизован в боте
                            
                            🛑В случае, если вы хотите отменить платеж, воспользуйтесь кнопкой "Связаться с админом" и напишите причину отмены.

                            🛑После оплаты воспользуйтесь кнопкой "Связаться с админом" и напишите "+" или любой другой символ.

                            🛑В случае оплаты после истечения времени Вы потеряете свои средства
                            """,Telegram.Bot.Types.Enums.ParseMode.Html);
                        return;
                    }
                    else
                        await sender
                        .EditMessageAsync(
                       chatId: message.Chat.Id,
                       messageId: message.MessageId,
                       text: "Сумма пишется цифрами Например 500");

                }

            }

            var keyboardButtons = message.ReplyMarkup!.InlineKeyboard.SelectMany(e => e).ToList();

            if (keyboardButtons.Count() <= _bank.Count)
                keyboardButtons.Add(InlineKeyboardButton.WithCallbackData("Подтвердить", nameof(DepositPage) + " con"));

            foreach (var button in keyboardButtons)
            {
                if (button.CallbackData.Contains(args[0].ToLower()))
                {
                    bool isOff = args[1] == "off";

                    button.Text = (isOff ? "✅" : "❌") + args[0].ToUpper();
                    button.CallbackData = string.Join(' ', nameof(DepositPage), args[0].ToLower(), isOff ? "on" : "off");

                    if (isOff)
                        keyboardButtons.Last().CallbackData += " " + args[0].ToLower();
                    else
                        keyboardButtons.Last().CallbackData = keyboardButtons.Last().CallbackData.Replace(" " + args[0].ToLower(), "");


                    if (keyboardButtons.Last().CallbackData == nameof(DepositPage) + " con")
                        keyboardButtons.Remove(keyboardButtons.Last());

                    break;
                }
            }

            try
            {
                await sender
               .EditReplyMarkupAsync(
               message.Chat.Id,
               message.MessageId,
               new InlineKeyboardMarkup(keyboardButtons.Select(r => new List<InlineKeyboardButton>() { r })));
            }
            catch (Exception)
            {
                // IF OLD == NEW --> Exception
            }
        }
    }
}
