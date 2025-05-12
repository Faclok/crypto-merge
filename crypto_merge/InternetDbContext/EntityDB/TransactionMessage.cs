using System.ComponentModel.DataAnnotations.Schema;

namespace InternetDatabase.EntityDB;

/// <summary>
/// Сообщения транзакции
/// </summary>
public class TransactionMessage : BaseEntity
{

    public string Message { get; private set; } = string.Empty;

    public DateTime DateTimeUTC { get; private set; } = DateTime.UtcNow;

    public int? TransactionId { get; private set; }

    [ForeignKey(nameof(TransactionId))]
    public TransactionWallet? TransactionWallet { get; private set; }

    public bool IsUserSender { get; private set; }

    public bool IsPhoto { get; private set; }

    public int? FileId { get; private set; }

    [ForeignKey(nameof(FileId))]
    public FileInfo? File { get; private set; }

    public int? BankId { get; private set; }

    [ForeignKey(nameof(BankId))]
    public Bank? Bank { get; private set; }

    public string? Tag { get; set; }

    public TransactionMessage(string message, int? transactionId, bool isUserSender, int? bankId, string? tag, FileInfo? file = null)
    {
        Message = message;
        DateTimeUTC = DateTime.UtcNow;
        TransactionId = transactionId;
        IsUserSender = isUserSender;
        BankId = bankId;
        Tag = tag;
        IsPhoto = file != null;
        File = file;
    }

    private TransactionMessage()
    {

    }
}
