namespace InternetDatabase.EntityDB
{
    public class Bank : BaseEntity
    {

        public required string Name { get; set; }

        public ICollection<TransactionWallet> TransactionWallets { get; set; } = [];

        public ICollection<TransactionMessage> TransactionMessages { get; set; } = [];
    }
}
