
namespace InternetDatabase.EntityDB
{
    public class AccountCryptoCard: BaseEntity
    {

        public string Name { get; set; } = string.Empty;

        public ICollection<Wallet> Wallets { get; set; } = [];
    }
}
