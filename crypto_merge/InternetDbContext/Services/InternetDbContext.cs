using InternetDatabase.EntityDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InternetDatabase.Services
{
    public class InternetDbContext : DbContext
    {
        #region Entity

        public DbSet<TelegramUser> Users { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<TransactionWallet> TransactionWallets { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<EntityDB.FileInfo> Files { get; set; }
        public DbSet<AccountCryptoCard> AccountCryptoCards { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        #endregion

        #region Configuration
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public InternetDbContext(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var stringConnect = _configuration.GetSection("DatabaseConnectionString").Value;
            optionsBuilder.UseMySql(stringConnect , new MySqlServerVersion(new Version(8, 0, 35)));
            optionsBuilder.UseLoggerFactory(_loggerFactory);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TransactionWallet>()
                .Property(t => t.Sum)
                .HasConversion<decimal>();

            modelBuilder.Entity<Currency>()
                .HasData(new Currency("USD", 92, "$")
                {
                    Id = 1
                });

            modelBuilder.Entity<Currency>()
                .HasQueryFilter(o => !o.SoftDeleted);

            modelBuilder.Entity<TelegramUser>()
                .HasQueryFilter(o => !o.SoftDeleted);

            modelBuilder.Entity<Bank>()
                .HasQueryFilter(o => !o.SoftDeleted);

            modelBuilder.Entity<TransactionWallet>()
                .HasQueryFilter(o => !o.SoftDeleted);

            modelBuilder.Entity<Wallet>()
                .HasQueryFilter(o => !o.SoftDeleted);

            modelBuilder.Entity<EntityDB.FileInfo>()
                .HasQueryFilter(o => !o.SoftDeleted);
    }
        #endregion
    }
}
