using System.ComponentModel.DataAnnotations.Schema;

namespace InternetDatabase.EntityDB
{

    /// <summary>
    /// Операция отвечающая за историю кошелька (банка)
    /// </summary>
    public class TransactionWallet : BaseEntity
    {

        /// <summary>
        /// Дата выполнения
        /// </summary>
        public DateTime DateTimeUTC { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Обязательное сообщение, согласно ТЗ
        /// </summary>
        public string Message { get; private set; } = string.Empty;

        /// <summary>
        /// ID выдает на платформе, обязателен только при работе с выкупом
        /// </summary>
        public string? TransactionIdCC { get; private set; }

        /// <summary>
        /// Сумма, если Deposit Type то руб, если Removal Type то USDT 
        /// </summary>
        public decimal Sum { get; set; }

        /// <summary>
        /// Тип транзакции, за реферала, вывод денет, внос денег
        /// </summary>
        public TransactionType Type { get; private set; }

        /// <summary>
        /// Статус транзакции, 
        /// </summary>
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// Кто обрабатывает заявку со стороны админа
        /// </summary>
        public int BankId { get; private set; }

        [ForeignKey(nameof(BankId))]
        public Bank? Bank { get; private set; }

        /// <summary>
        /// Банки слитые в одну строку используя сепаратор
        /// </summary>
        public string Banks { get; private set; } = "";

        /// <summary>
        /// Сепаратор используется для слития и разделения банков
        /// </summary>
        public string Separator { get; private set; } = ";";

        /// <summary>
        /// С каким кошельком происходит работа
        /// </summary>
        public int? WalletId { get; private set; }

        [ForeignKey(nameof(WalletId))]
        public Wallet? Wallet { get; private set; }

        [NotMapped]
        public const string SEPARATOR = ";";

        public ICollection<TransactionMessage> Chat { get; private set; } = [];

        public TransactionWallet(int bankId, int walletId, decimal countMoney, string message, TransactionStatus status, TransactionType type)
            => (BankId, WalletId, Sum, Message, Status, Type) = (bankId, walletId, countMoney, message, status, type);

        public TransactionWallet(int bankId, int walletId, decimal countMoney, string message, string cc)
            => (BankId, WalletId, Sum, Message, Status, Type, TransactionIdCC) = (bankId, walletId, countMoney, message, TransactionStatus.Completed, TransactionType.Removal, cc);

        public TransactionWallet(int bankId, int walletId, decimal countMoney, string message, string cc, DateTime dateTime)
    => (BankId, WalletId, Sum, Message, Status, Type, TransactionIdCC, DateTimeUTC) = (bankId, walletId, countMoney, message, TransactionStatus.Completed, TransactionType.Removal, cc, dateTime);

        public TransactionWallet(int bankId, int walletId, decimal countMoney, string message, string[] banks)
            => (BankId, WalletId, Sum, Message, Status, Type, Banks, Separator) = (bankId, walletId, countMoney, message, TransactionStatus.Wait, TransactionType.Deposit, string.Join(SEPARATOR, banks), SEPARATOR);

        /// <summary>
        /// transfer to wallet
        /// </summary>
        /// <param name="bankId"></param>
        /// <param name="walletId"></param>
        /// <param name="countMoney"></param>
        /// <param name="message"></param>
        public TransactionWallet(int bankId, int walletId, decimal countMoney, string message, TransactionType type)
            => (BankId, WalletId, Sum, Message, Status, Type) = (bankId, walletId, countMoney, message, TransactionStatus.Completed, type);

        private TransactionWallet() { }
    }

    public enum TransactionType
    {
        None, Referral, Deposit, Removal, TransferToWallet, TransferFromWallet
    }

    public enum TransactionStatus
    {
        /// <summary>
        /// НЕ МЕНЯТЬ МЕСТАМИ
        /// </summary>
        None, Wait, InProgress, Completed
    }
}
