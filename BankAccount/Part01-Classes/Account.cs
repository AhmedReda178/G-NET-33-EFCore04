using System;
using System.Collections.Generic;
using System.Text;

namespace BankAccount.Part01_Classes
{
    public enum AccountType { Savings, Current, Business }

    public class Account
    {
        public string AccountNumber { get; set; }   // PK
        public AccountType AccountType { get; set; }
        public DateTime OpeningDate { get; set; }
        public decimal CurrentBalance { get; set; }

        // FK
        public int BranchCode { get; set; }
        public Branch Branch { get; set; }

        // Navigation
        public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
