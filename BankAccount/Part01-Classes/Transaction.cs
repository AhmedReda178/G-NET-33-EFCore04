using System;
using System.Collections.Generic;
using System.Text;

namespace BankAccount.Part01_Classes
{
    public enum TransactionType { Deposit, Withdrawal, Transfer, Payment }

    public class Transaction
    {
        public int TransactionNumber { get; set; }   // PK
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Note { get; set; }

        // FK
        public string AccountNumber { get; set; }
        public Account Account { get; set; }
    }
}
