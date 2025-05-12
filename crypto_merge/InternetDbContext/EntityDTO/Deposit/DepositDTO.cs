namespace InternetDatabase.EntityDTO.Deposit
{

    /// <summary>
    /// Заявка на откуп
    /// </summary>
    public class DepositDTO
    {
        public required long ChatId { get; set; }

        public string? Username { get; set; }

        public string? CryptoCardId { get; set; }

        public required int TransactionId { get; set; }

        public required decimal CountTransaction { get; set; }

        public string[] Banks { get; set; } = [];

        public required decimal Balance { get; set; }

        public decimal BalanceBoost { get; set; }

        public MessageDTO[] Chat { get; set; } = [];

    }
}
