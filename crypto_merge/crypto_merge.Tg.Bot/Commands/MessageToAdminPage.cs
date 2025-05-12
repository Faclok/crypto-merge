using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands.Abstractions;
using crypto_merge.Tg.Bot.Services;
using Telegram.Bot;                                    
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands;

internal class MessageToAdminPage(MessageSender messageSender,MessageService messageService,WaitForUserResponse waitForUser, ITelegramBotClient botClient): ICallbackCommand
{
    public string CallbackKey => nameof(MessageToAdminPage);

    public async Task Handler(CallbackQuery callbackQuery, string[] args)
    {
        try
        {
            await botClient.EditMessageReplyMarkupAsync(callbackQuery.InlineMessageId);
        }
        catch (Exception)
        {
            await messageSender.SendMessageAsync(callbackQuery.Message.Chat, callbackQuery.Message.Text);
            await messageSender.DeleteMessageAsync(callbackQuery.Message.Chat, callbackQuery.Message.MessageId);
        }

        await messageSender.SendMessageAsync(callbackQuery.Message.Chat.Id, "Напишите ваше сообщение и отправьте его боту");

        while (true)
        {
            var message = await waitForUser.GetMessageAsync(callbackQuery.Message.Chat.Id);

            if (message is null)
            {
                await messageSender.SendMessageAsync(callbackQuery.Message.Chat.Id, "Ответе в начале админу");
                continue;
            }
            switch (message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Text:
                    await messageService.SendFromUser(int.Parse(args[0]), message.Text ?? "");
                    return;
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                    await messageService.SendFromUser(int.Parse(args[0]), message.Caption ?? "", await DownloadFile(message.Photo[1].FileId));
                    return;
                
                default:
                    await messageSender.SendMessageAsync(callbackQuery.Message.Chat.Id, "Админ принимает текстовые или сообщение с одним фото");
                    break;
            }
        }

    }

    public async Task<InternetDatabase.EntityDB.FileInfo> DownloadFile(string fileId)
    {

        var fileInfo = await botClient.GetFileAsync(fileId);

        var fileFullName = fileInfo.FilePath.Split("/").Last();
        var fileName = fileFullName.Split(".").First();
        var fileExt = fileFullName.Split(".").Last();

        using var ms = new MemoryStream();
        await botClient.DownloadFileAsync(fileInfo.FilePath, ms);
        ms.Seek(0, SeekOrigin.Begin);

        return new InternetDatabase.EntityDB.FileInfo()
        {
            Data = ms.ToArray(),
            Extension = fileExt,
            Name = fileName,
        };
    }
}
