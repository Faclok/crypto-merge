using System.ComponentModel.DataAnnotations.Schema;

namespace InternetDatabase.EntityDB
{

    /// <summary>
    /// Пользователь
    /// </summary>
    public class TelegramUser : BaseEntity
    {

        /// <summary>
        /// Пользовательский ChatId основного бота
        /// </summary>
        public long ChatId { get; private set; }

        /// <summary>
        /// Username берется из телеграмма
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Полное имя берется из телеграмма
        /// </summary>
        public string FullName { get; set; } = "noname";

        /// <summary>
        /// Временный баланс
        /// </summary>
        public decimal BalanceTemp { get; set; }

        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime RegistrationDate { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Реферал от которого пришел этот пользователь
        /// </summary>
        public int? CameFromTelegramUserId { get; private set; }

        [ForeignKey(nameof(CameFromTelegramUserId))]
        public TelegramUser? CameFromTelegramUser { get; private set; }

        /// <summary>
        /// Рефералы кого пригласил этот пользователь
        /// </summary>
        public ICollection<TelegramUser> ReferralTelegramUsers { get; set; } = [];

        /// <summary>
        /// Все привязанные банки
        /// </summary>
        public ICollection<Wallet> Wallets { get; set; } = [];
        public string? NoteAdmin { get; set; }

        public TelegramUser(long chatId, int? referralUserId)
        {
            ChatId = chatId;
            CameFromTelegramUserId = referralUserId;
        }

        private TelegramUser() { }
    }
}
