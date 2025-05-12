using System.ComponentModel.DataAnnotations.Schema;

namespace InternetDatabase.EntityDB;

/// <summary>
/// Привязанный банк к пользователю
/// </summary>
public class Wallet : BaseEntity
{

    /// <summary>
    /// ФИО
    /// </summary>
    public required string Fio { get; set; }

    /// <summary>
    /// Номер карты
    /// </summary>
    public required string NumberCard { get; set; }

    /// <summary>
    /// ID в системе CryptoCard
    /// </summary>
    public string CryptoCardId { get; set; } = string.Empty;

    /// <summary>
    /// Статут банка
    /// </summary>
    public WalletStatus Status { get; set; }

    /// <summary>
    /// Название банка
    /// </summary>
    public required string Bank { get; set; }

    public decimal LimitFix { get; set; }
    /// <summary>
    /// Временная прибавка к балансу
    /// </summary>
    public decimal BalanceBoost { get; set; }

    public bool IsCheckYesterday { get; set; } = false;

    /// <summary>
    /// Телефон
    /// </summary>
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// Логин от банка
    /// </summary>
    public required string Login { get; set; }

    /// <summary>
    /// Пароль от банка
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Чей это кошелек
    /// </summary>
    public int? TelegramUserId { get; set; }

    [ForeignKey(nameof(TelegramUserId))]
    public TelegramUser? TelegramUser { get; set; }

    public int? AccountCryptoCardId { get; set;}

    [ForeignKey(nameof(AccountCryptoCardId))]
    public AccountCryptoCard? AccountCryptoCard { get; set; }


    /// <summary>
    /// История транзакций
    /// </summary>
    public ICollection<TransactionWallet> TransactionWallets { get; set; } = [];

}

public enum WalletStatus
{
    Connection,
    Connected,
    Stopping,
    Stop,
}
