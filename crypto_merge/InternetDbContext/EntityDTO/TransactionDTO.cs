using InternetDatabase.EntityDB;

namespace InternetDatabase.EntityDTO
{

    /// <summary>
    /// Операция отвечающая за историю кошелька (банка)
    /// </summary>
    public class TransactionDTO : BaseEntity
    {
        /// <summary>
        /// Дата выполнения
        /// </summary>
        public DateTime DateTimeUTC { get; set; }

        /// <summary>
        /// Обязательное сообщение, согласно ТЗ
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Сумма, если Deposit Type то руб, если Removal Type то USDT 
        /// </summary>
        public required decimal Count { get; set; }

        /// <summary>
        /// Тип транзакции, за реферала, вывод денет, внос денег
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Статус транзакции, 
        /// </summary>
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// Кто обрабатывает заявку со стороны админа
        /// </summary>
        public required int BankId { get; set; }

        /// <summary>
        /// С каким кошельком происходит работа
        /// </summary>
        public int? WalletId { get; set; }
    }
}
