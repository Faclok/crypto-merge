using InternetDatabase.EntityDB;
using InternetDatabase.EntityDTO.Deposit;
using InternetDatabase.Services;
using Microsoft.EntityFrameworkCore;
using FileInfo = InternetDatabase.EntityDB.FileInfo;

namespace BusLogic.Services
{
    public class MessageService(InternetDbContext context)
    {
        public Task<MessageDTO[]> GetByTransactionIdAsync(int transactionId)
        {
            return context.TransactionWallets
                .Where(t=>t.Id == transactionId)
                .SelectMany(t => t.Chat)
                .Include(m=>m.File)
                .Select(t => new MessageDTO()
                {
                    Message = t.Message,
                    DateTime = t.DateTimeUTC,
                    File = t.File,
                    IsUser = t.IsUserSender,
                    Tag = t.Tag,
                })
                .ToArrayAsync();
        }

        public async Task<int> SendFromAdmin(int transactionId, string message, string? tag)
        {
            if (await context.TransactionWallets
               .Include(o => o.Chat)
               .FirstOrDefaultAsync(o => o.Id == transactionId) is not { } transaction)
                return 0;

            var transactionMessage = new TransactionMessage(message, transactionId, false, 1, tag);
            transaction.Chat.Add(transactionMessage);
            await context.SaveChangesAsync();

            return transactionMessage.Id;
        }

        public async Task SendFromUser(int transactionId, string message, FileInfo? file = null)
        {
            if (await context.TransactionWallets
                .Include(o => o.Chat)
                .FirstOrDefaultAsync(o => o.Id == transactionId) is not { } transaction)
                return;

            transaction.Chat.Add(new(message, transactionId, true, 1, null,file));
            await context.SaveChangesAsync();
        }
    }
}
