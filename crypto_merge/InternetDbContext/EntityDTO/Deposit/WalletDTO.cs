
namespace InternetDatabase.EntityDTO.Deposit;

public class WalletDTO
{
    public int Id { get; set; }
    public long? ChatId { get; set; }

    public required string Phone { get; set; }

    public required string NumberCard { get; set; }

    public decimal Balance { get; set; }

    public int? AccountCryptoCardId { get; set; }
}
