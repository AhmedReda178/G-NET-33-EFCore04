using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankAccount.Part01_Classes
{
    public class BankDbContext : DbContext
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=.;Database=NationalBank;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Branch - Manager (1:1)
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Manager)
                .WithOne(m => m.Branch)
                .HasForeignKey<Manager>(m => m.BranchCode);

            // Branch PK
            modelBuilder.Entity<Branch>()
                .HasKey(b => b.Code);

            // Account PK
            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountNumber);

            // Transaction PK
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionNumber);

            // CustomerAccount Composite Key
            modelBuilder.Entity<CustomerAccount>()
                .HasKey(ca => new { ca.CustomerId, ca.AccountNumber });

            // Relationships
            modelBuilder.Entity<CustomerAccount>()
                .HasOne(ca => ca.Customer)
                .WithMany(c => c.CustomerAccounts)
                .HasForeignKey(ca => ca.CustomerId);

            modelBuilder.Entity<CustomerAccount>()
                .HasOne(ca => ca.Account)
                .WithMany(a => a.CustomerAccounts)
                .HasForeignKey(ca => ca.AccountNumber);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Branch)
                .WithMany(b => b.Accounts)
                .HasForeignKey(a => a.BranchCode);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountNumber);

            // Seeding
            modelBuilder.Entity<Branch>().HasData(
                new Branch { Code = 1, Name = "Cairo Main Branch", Address = "123 Tahrir St, Cairo", PhoneNumber = "02-1234567" },
                new Branch { Code = 2, Name = "Alexandria Branch", Address = "45 Corniche Rd, Alexandria", PhoneNumber = "03-9876543" }
            );

            modelBuilder.Entity<Manager>().HasData(
                new Manager { Id = 1, FullName = "Ahmed Hassan", Email = "ahmed@bank.com", PhoneNumber = "01001234567", HireDate = new DateTime(2020, 1, 15), BranchCode = 1 },
                new Manager { Id = 2, FullName = "Sara Mohamed", Email = "sara@bank.com", PhoneNumber = "01009876543", HireDate = new DateTime(2021, 3, 20), BranchCode = 2 }
            );
        }
    }
}
