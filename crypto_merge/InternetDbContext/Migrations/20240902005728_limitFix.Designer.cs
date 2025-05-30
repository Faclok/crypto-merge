﻿// <auto-generated />
using System;
using InternetDatabase.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InternetDatabase.Migrations
{
    [DbContext(typeof(InternetDbContext))]
    [Migration("20240902005728_limitFix")]
    partial class limitFix
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("InternetDatabase.EntityDB.Bank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Banks");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CharCurrency")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("RubCurrency")
                        .HasColumnType("decimal(65,30)");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Currencies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CharCurrency = "$",
                            RubCurrency = 92m,
                            SoftDeleted = false,
                            Title = "USD"
                        });
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.FileInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.TelegramUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CameFromTelegramUserId")
                        .HasColumnType("int");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NoteAdmin")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Username")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CameFromTelegramUserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.TransactionMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BankId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTimeUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("FileId")
                        .HasColumnType("int");

                    b.Property<bool>("IsPhoto")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsUserSender")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Tag")
                        .HasColumnType("longtext");

                    b.Property<int?>("TransactionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.HasIndex("FileId");

                    b.HasIndex("TransactionId");

                    b.ToTable("TransactionMessage");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.TransactionWallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BankId")
                        .HasColumnType("int");

                    b.Property<string>("Banks")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DateTimeUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Separator")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal>("Sum")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("TransactionIdCC")
                        .HasColumnType("longtext");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int?>("WalletId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.HasIndex("WalletId");

                    b.ToTable("TransactionWallets");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.Wallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Bank")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Fio")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsCheckYesterday")
                        .HasColumnType("tinyint(1)");

                    b.Property<decimal>("LimitFix")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NumberCart")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int?>("TelegramUserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TelegramUserId");

                    b.ToTable("Wallets");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.TelegramUser", b =>
                {
                    b.HasOne("InternetDatabase.EntityDB.TelegramUser", "CameFromTelegramUser")
                        .WithMany("ReferralTelegramUsers")
                        .HasForeignKey("CameFromTelegramUserId");

                    b.Navigation("CameFromTelegramUser");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.TransactionMessage", b =>
                {
                    b.HasOne("InternetDatabase.EntityDB.Bank", "Bank")
                        .WithMany("TransactionMessages")
                        .HasForeignKey("BankId");

                    b.HasOne("InternetDatabase.EntityDB.FileInfo", "File")
                        .WithMany()
                        .HasForeignKey("FileId");

                    b.HasOne("InternetDatabase.EntityDB.TransactionWallet", "TransactionWallet")
                        .WithMany("Chat")
                        .HasForeignKey("TransactionId");

                    b.Navigation("Bank");

                    b.Navigation("File");

                    b.Navigation("TransactionWallet");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.TransactionWallet", b =>
                {
                    b.HasOne("InternetDatabase.EntityDB.Bank", "Bank")
                        .WithMany("TransactionWallets")
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InternetDatabase.EntityDB.Wallet", "Wallet")
                        .WithMany("TransactionWallets")
                        .HasForeignKey("WalletId");

                    b.Navigation("Bank");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.Wallet", b =>
                {
                    b.HasOne("InternetDatabase.EntityDB.TelegramUser", "TelegramUser")
                        .WithMany("Wallets")
                        .HasForeignKey("TelegramUserId");

                    b.Navigation("TelegramUser");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.Bank", b =>
                {
                    b.Navigation("TransactionMessages");

                    b.Navigation("TransactionWallets");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.TelegramUser", b =>
                {
                    b.Navigation("ReferralTelegramUsers");

                    b.Navigation("Wallets");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.TransactionWallet", b =>
                {
                    b.Navigation("Chat");
                });

            modelBuilder.Entity("InternetDatabase.EntityDB.Wallet", b =>
                {
                    b.Navigation("TransactionWallets");
                });
#pragma warning restore 612, 618
        }
    }
}
